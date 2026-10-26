namespace Business.Contracts.Telegram;

public sealed record TelegramAuthorizationResult(
    Guid AttemptId,
    TelegramAuthorizationStep Step,
    DateTimeOffset ExpiresAt,
    int? AgentId,
    bool InputRejected);