using Business.Contracts.Telegram;

namespace Business.Abstractions.Telegram;

public interface ITelegramAuthorizationClient : IAsyncDisposable
{
    long UserId { get; }

    Task<TelegramAuthorizationStep> SubmitAsync(string value, CancellationToken ct);

    byte[] GetSessionData();
}
