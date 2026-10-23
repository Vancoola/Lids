namespace Domain.Agents;

public abstract class AgentConnection
{
    public Guid Id { get; protected set; }
    public abstract Platform Platform { get; }
}

public sealed class TelegramConnection : AgentConnection
{
    private TelegramConnection()
    {
    }

    public string SessionKey { get; private set; } = null!;

    public long UserId { get; private set; }

    public override Platform Platform { get; } = Platform.Telegram;

    public static TelegramConnection Create(
        string sessionKey,
        long userId)
    {
        return new TelegramConnection
        {
            Id = Guid.CreateVersion7(),
            SessionKey = sessionKey,
            UserId = userId
        };
    }
}