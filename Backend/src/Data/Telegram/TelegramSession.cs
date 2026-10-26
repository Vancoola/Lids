namespace Data.Telegram;

public sealed class TelegramSession
{
    private TelegramSession()
    {
    }

    public string Key { get; private set; } = null!;
    public byte[] Data { get; private set; } = null!;
    public DateTime CreatedAt { get; private set; }

    public static TelegramSession Create(string key, byte[] data)
    {
        return new TelegramSession
        {
            Key = key,
            Data = data.ToArray(),
            CreatedAt = DateTime.UtcNow
        };
    }
}
