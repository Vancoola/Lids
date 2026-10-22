namespace Domain.Agents;

public abstract class AgentConnection
{
    protected AgentConnection() { }

    public Guid Id { get; protected set; }
    public abstract Platform Platform { get; }
}

public sealed class TelegramConnection : AgentConnection
{
    private TelegramConnection() { }

    public string SessionKey { get; private set; } = null!;

    public long UserId { get; private set; }

    public static TelegramConnection Create(
        string sessionKey,
        long userId)
        => new()
        {
            Id = Guid.CreateVersion7(),
            SessionKey = sessionKey,
            UserId = userId
        };

    public override Platform Platform { get; } = Platform.Telegram;
}