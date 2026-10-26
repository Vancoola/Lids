using Business.Contracts.Telegram;

namespace Business.Abstractions.Telegram;

public interface ITelegramAuthorizationService
{
    Task<TelegramAuthorizationResult> StartAsync(Guid ownerId, string phoneNumber, CancellationToken ct);

    Task<TelegramAuthorizationResult> ContinueAsync(
        Guid ownerId,
        Guid attemptId,
        TelegramAuthorizationStep step,
        string value,
        CancellationToken ct);

    Task<TelegramAuthorizationResult> GetAsync(Guid ownerId, Guid attemptId, CancellationToken ct);

    Task<TelegramAuthorizationResult> CompleteAsync(Guid ownerId, Guid attemptId, CancellationToken ct);

    Task CancelAsync(Guid ownerId, Guid attemptId, CancellationToken ct);

    Task RemoveExpiredAsync(CancellationToken ct);
}
