namespace Business.Contracts.Messaging;

public sealed record IncomingMessage(
    int AgentId,
    string ExternalChatId,
    string ExternalMessageId,
    string ExternalSenderId,
    string Text,
    DateTimeOffset SentAt
);