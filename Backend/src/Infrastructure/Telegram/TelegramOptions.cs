namespace Infrastructure.Telegram;

public sealed class TelegramOptions
{
    public int ApiId { get; set; }
    public string ApiHash { get; set; } = string.Empty;
}
