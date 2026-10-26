namespace Business.Abstractions.Telegram;

public interface ITelegramAuthorizationClientFactory
{
    ITelegramAuthorizationClient Create();
}
