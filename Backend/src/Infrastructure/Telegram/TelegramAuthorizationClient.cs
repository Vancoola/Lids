using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using Business.Abstractions.Telegram;
using Business.Contracts.Telegram;
using Business.Exceptions;
using TL;

namespace Infrastructure.Telegram;

public sealed class TelegramAuthorizationClient : ITelegramAuthorizationClient
{
    private readonly WTelegram.Client _client;
    private readonly TelegramOptions _options;
    private readonly CancellationTokenSource _lifetime = new();
    private readonly object _sessionLock = new();
    private TaskCompletionSource<TelegramAuthorizationStep> _nextStep = NewStep();
    private TaskCompletionSource<string>? _answer;
    private Task? _loginTask;
    private string? _phoneNumber;
    private byte[] _sessionData = [];
    private bool _disposed;

    static TelegramAuthorizationClient()
    {
        // Диагностические сообщения библиотеки могут содержать данные авторизации.
        WTelegram.Helpers.Log = (_, _) => { };
    }

    public TelegramAuthorizationClient(TelegramOptions options)
    {
        _options = options;
        _client = new WTelegram.Client(Config, null, SaveSession)
        {
            FloodRetryThreshold = 0
        };
        _client.TcpHandler = ConnectAsync;
    }

    public long UserId => _client.UserId;

    public async Task<TelegramAuthorizationStep> SubmitAsync(string value, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        if (_loginTask is null)
        {
            _phoneNumber = value;
            _loginTask = LoginAsync();
        }
        else
        {
            _nextStep = NewStep();
            _answer!.TrySetResult(value);
        }

        return await _nextStep.Task.WaitAsync(ct);
    }

    public byte[] GetSessionData()
    {
        lock (_sessionLock)
            return _sessionData.ToArray();
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposed)
            return;

        _disposed = true;
        await _lifetime.CancelAsync();
        try
        {
            await _client.DisposeAsync();
            if (_loginTask is not null)
                await _loginTask;
        }
        finally
        {
            _phoneNumber = null;
            _answer = null;
            lock (_sessionLock)
            {
                CryptographicOperations.ZeroMemory(_sessionData);
                _sessionData = [];
            }

            _lifetime.Dispose();
        }
    }

    private async Task LoginAsync()
    {
        try
        {
            await _client.LoginUserIfNeeded();
            _nextStep.TrySetResult(TelegramAuthorizationStep.Authorized);
        }
        catch (OperationCanceledException)
        {
            _nextStep.TrySetCanceled();
        }
        catch (Exception exception)
        {
            _nextStep.TrySetException(MapError(exception));
        }
    }

    private string? Config(string name)
    {
        return name switch
        {
            "api_id" => _options.ApiId.ToString(CultureInfo.InvariantCulture),
            "api_hash" => _options.ApiHash,
            "phone_number" => _phoneNumber,
            "verification_code" => RequestInput(TelegramAuthorizationStep.Code),
            "password" => RequestInput(TelegramAuthorizationStep.Password),
            "email" => RequestInput(TelegramAuthorizationStep.Email),
            "email_verification_code" => RequestInput(TelegramAuthorizationStep.EmailCode),
            "first_name" or "last_name" => throw new TelegramAuthorizationException(
                "Only existing Telegram accounts can be connected. Create the account in Telegram first."),
            _ => null
        };
    }

    private string RequestInput(TelegramAuthorizationStep step)
    {
        _lifetime.Token.ThrowIfCancellationRequested();
        var answer = new TaskCompletionSource<string>(TaskCreationOptions.RunContinuationsAsynchronously);
        _answer = answer;
        _nextStep.TrySetResult(step);

        // WTelegram запрашивает значения синхронно. Отмена освобождает ожидание при истечении попытки.
        try
        {
            return answer.Task.WaitAsync(_lifetime.Token).GetAwaiter().GetResult();
        }
        finally
        {
            Interlocked.CompareExchange(ref _answer, null, answer);
        }
    }

    private async Task<TcpClient> ConnectAsync(string host, int port)
    {
        var client = new TcpClient();
        try
        {
            await client.ConnectAsync(host, port, _lifetime.Token);
            return client;
        }
        catch
        {
            client.Dispose();
            throw;
        }
    }

    private void SaveSession(byte[] data)
    {
        lock (_sessionLock)
        {
            CryptographicOperations.ZeroMemory(_sessionData);
            _sessionData = data.ToArray();
        }
    }

    private static TaskCompletionSource<TelegramAuthorizationStep> NewStep()
    {
        return new TaskCompletionSource<TelegramAuthorizationStep>(TaskCreationOptions.RunContinuationsAsynchronously);
    }

    private static Exception MapError(Exception exception)
    {
        if (exception is BusinessException)
            return exception;

        if (exception is RpcException rpc)
        {
            if (rpc.Code == 420)
                return new TelegramAuthorizationException(
                    "Telegram temporarily limited authorization requests. Try again later.", HttpStatusCode.TooManyRequests);

            return rpc.Message switch
            {
                "PHONE_NUMBER_INVALID" => new TelegramAuthorizationException("Phone number is invalid."),
                "PHONE_NUMBER_BANNED" => new TelegramAuthorizationException("This phone number is banned by Telegram."),
                "PHONE_CODE_EXPIRED" => new TelegramAuthorizationException("The code has expired. Start a new authorization."),
                "PHONE_CODE_INVALID" or "PASSWORD_HASH_INVALID" or "CODE_INVALID" => new TelegramAuthorizationException(
                    "Too many incorrect authorization values. Start a new authorization."),
                "API_ID_INVALID" => new TelegramAuthorizationException("Telegram API credentials are invalid.",
                    HttpStatusCode.ServiceUnavailable),
                _ => new TelegramAuthorizationException("Telegram rejected authorization. Start a new authorization.")
            };
        }

        return new TelegramAuthorizationException("Unable to authorize with Telegram. Start a new authorization.",
            HttpStatusCode.BadGateway);
    }
}
