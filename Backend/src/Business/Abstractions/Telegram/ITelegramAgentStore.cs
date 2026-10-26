namespace Business.Abstractions.Telegram;

public interface ITelegramAgentStore
{
    Task<int> CreateAsync(Guid attemptId, long userId, byte[] sessionData, CancellationToken ct);
}