using System.Text.Json.Serialization;

namespace Business.Contracts.Telegram;

[JsonConverter(typeof(JsonStringEnumConverter<TelegramAuthorizationStep>))]
public enum TelegramAuthorizationStep
{
    PhoneNumber,
    Code,
    Password,
    Email,
    EmailCode,
    Authorized,
    Completed
}
