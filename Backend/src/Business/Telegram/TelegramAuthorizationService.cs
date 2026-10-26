using System.Collections.Concurrent;
using System.Net;
using System.Security.Cryptography;
using Business.Abstractions.Telegram;
using Business.Contracts.Telegram;
using Business.Exceptions;

namespace Business.Telegram;

public sealed class TelegramAuthorizationService(
    ITelegramAuthorizationClientFactory clientFactory,
    ITelegramAgentStore store,
    TimeProvider timeProvider) : ITelegramAuthorizationService, IAsyncDisposable
{
    private static readonly TimeSpan AttemptLifetime = TimeSpan.FromMinutes(10);
    private static readonly TimeSpan RequestTimeout = TimeSpan.FromSeconds(30);
    private readonly ConcurrentDictionary<Guid, AuthorizationAttempt> _attempts = new();
    private readonly object _sync = new();

    public async Task<TelegramAuthorizationResult> StartAsync(
        Guid ownerId,
        string phoneNumber,
        CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber) || phoneNumber.Length is < 8 or > 16 ||
            phoneNumber[0] != '+' || phoneNumber[1] == '0' ||
            phoneNumber.AsSpan(1).ContainsAnyExceptInRange('0', '9'))
            throw new TelegramAuthorizationException("Phone number must use international format, for example +79991234567.");

        AuthorizationAttempt attempt;
        lock (_sync)
        {
            if (_attempts.Count >= 100 || _attempts.Values.Count(a => a.OwnerId == ownerId && a.AgentId is null) >= 2)
                throw new TelegramAuthorizationException(
                    "Too many authorization attempts. Complete or cancel an existing attempt.",
                    HttpStatusCode.TooManyRequests);

            attempt = new AuthorizationAttempt(ownerId, timeProvider.GetUtcNow().Add(AttemptLifetime),
                clientFactory.Create());
            _attempts[attempt.Id] = attempt;
        }

        return await ContinueAsync(ownerId, attempt.Id, TelegramAuthorizationStep.PhoneNumber, phoneNumber, ct);
    }

    public async Task<TelegramAuthorizationResult> ContinueAsync(
        Guid ownerId,
        Guid attemptId,
        TelegramAuthorizationStep step,
        string value,
        CancellationToken ct)
    {
        var attempt = FindAttempt(ownerId, attemptId);
        await attempt.Gate.WaitAsync(ct);
        try
        {
            EnsureActive(attempt);

            if (attempt.Step is TelegramAuthorizationStep.Authorized or TelegramAuthorizationStep.Completed)
                return await CompleteAsync(attempt, ct);

            if (step != attempt.Step)
                throw new TelegramAuthorizationException("The authorization step has changed. Read the current state.",
                    HttpStatusCode.Conflict);

            if (string.IsNullOrWhiteSpace(value) || value.Length > 4096)
                throw new TelegramAuthorizationException("Authorization value is required and must not exceed 4096 characters.");

            try
            {
                using var timeout = new CancellationTokenSource(RequestTimeout, timeProvider);
                using var linked = CancellationTokenSource.CreateLinkedTokenSource(ct, timeout.Token);
                attempt.Step = await attempt.Client!.SubmitAsync(value, linked.Token);
                attempt.InputRejected = attempt.Step == step;
            }
            catch (OperationCanceledException) when (!ct.IsCancellationRequested)
            {
                await CloseAsync(attempt);
                throw new TelegramAuthorizationException("Telegram did not respond in time. Start a new authorization.",
                    HttpStatusCode.GatewayTimeout);
            }
            catch
            {
                await CloseAsync(attempt);
                throw;
            }

            if (attempt.Step == TelegramAuthorizationStep.Authorized)
                return await CompleteAsync(attempt, ct);

            return Result(attempt);
        }
        finally
        {
            attempt.Gate.Release();
        }
    }

    public async Task<TelegramAuthorizationResult> GetAsync(Guid ownerId, Guid attemptId, CancellationToken ct)
    {
        var attempt = FindAttempt(ownerId, attemptId);
        await attempt.Gate.WaitAsync(ct);
        try
        {
            EnsureActive(attempt);
            return Result(attempt);
        }
        finally
        {
            attempt.Gate.Release();
        }
    }

    public async Task<TelegramAuthorizationResult> CompleteAsync(Guid ownerId, Guid attemptId, CancellationToken ct)
    {
        var attempt = FindAttempt(ownerId, attemptId);
        await attempt.Gate.WaitAsync(ct);
        try
        {
            EnsureActive(attempt);
            if (attempt.Step is not (TelegramAuthorizationStep.Authorized or TelegramAuthorizationStep.Completed))
                throw new TelegramAuthorizationException("Telegram authorization is not complete.", HttpStatusCode.Conflict);

            return await CompleteAsync(attempt, ct);
        }
        finally
        {
            attempt.Gate.Release();
        }
    }

    public async Task CancelAsync(Guid ownerId, Guid attemptId, CancellationToken ct)
    {
        var attempt = FindAttempt(ownerId, attemptId);
        await attempt.Gate.WaitAsync(ct);
        try
        {
            await CloseAsync(attempt);
        }
        finally
        {
            attempt.Gate.Release();
        }
    }

    public async Task RemoveExpiredAsync(CancellationToken ct)
    {
        foreach (var attempt in _attempts.Values.Where(a => a.ExpiresAt <= timeProvider.GetUtcNow()))
        {
            if (!await attempt.Gate.WaitAsync(0, ct))
                continue;

            try
            {
                await CloseAsync(attempt);
            }
            finally
            {
                attempt.Gate.Release();
            }
        }
    }

    public async ValueTask DisposeAsync()
    {
        foreach (var attempt in _attempts.Values)
        {
            await attempt.Gate.WaitAsync();
            try
            {
                await CloseAsync(attempt);
            }
            finally
            {
                attempt.Gate.Release();
            }
        }
    }

    private async Task<TelegramAuthorizationResult> CompleteAsync(AuthorizationAttempt attempt, CancellationToken ct)
    {
        if (attempt.AgentId is not null)
            return Result(attempt);

        var client = attempt.Client!;
        var sessionData = client.GetSessionData();
        try
        {
            attempt.AgentId = await store.CreateAsync(attempt.Id, client.UserId, sessionData, ct);
        }
        catch (TelegramAccountAlreadyConnectedException)
        {
            await CloseAsync(attempt);
            throw;
        }
        finally
        {
            CryptographicOperations.ZeroMemory(sessionData);
        }

        attempt.Step = TelegramAuthorizationStep.Completed;
        attempt.InputRejected = false;
        attempt.Client = null;
        await client.DisposeAsync();
        return Result(attempt);
    }

    private AuthorizationAttempt FindAttempt(Guid ownerId, Guid attemptId)
    {
        if (!_attempts.TryGetValue(attemptId, out var attempt) || attempt.OwnerId != ownerId)
            throw new TelegramAuthorizationNotFoundException();

        return attempt;
    }

    private void EnsureActive(AuthorizationAttempt attempt)
    {
        if (attempt.Closed || attempt.ExpiresAt <= timeProvider.GetUtcNow())
            throw new TelegramAuthorizationNotFoundException();
    }

    private async Task CloseAsync(AuthorizationAttempt attempt)
    {
        _attempts.TryRemove(attempt.Id, out _);
        attempt.Closed = true;
        var client = attempt.Client;
        attempt.Client = null;
        if (client is not null)
            await client.DisposeAsync();
    }

    private static TelegramAuthorizationResult Result(AuthorizationAttempt attempt)
    {
        return new TelegramAuthorizationResult(attempt.Id, attempt.Step, attempt.ExpiresAt,
            attempt.AgentId, attempt.InputRejected);
    }

    private sealed class AuthorizationAttempt(
        Guid ownerId,
        DateTimeOffset expiresAt,
        ITelegramAuthorizationClient client)
    {
        public Guid Id { get; } = Guid.CreateVersion7();
        public Guid OwnerId { get; } = ownerId;
        public DateTimeOffset ExpiresAt { get; } = expiresAt;
        public SemaphoreSlim Gate { get; } = new(1, 1);
        public ITelegramAuthorizationClient? Client { get; set; } = client;
        public TelegramAuthorizationStep Step { get; set; } = TelegramAuthorizationStep.PhoneNumber;
        public int? AgentId { get; set; }
        public bool InputRejected { get; set; }
        public bool Closed { get; set; }
    }
}
