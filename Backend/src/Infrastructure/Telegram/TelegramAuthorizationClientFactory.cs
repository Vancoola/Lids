using System.Net;
using Business.Abstractions.Telegram;
using Business.Exceptions;
using Microsoft.Extensions.Options;

namespace Infrastructure.Telegram;

public sealed class TelegramAuthorizationClientFactory(IOptions<TelegramOptions> options)
    : ITelegramAuthorizationClientFactory
{
    public ITelegramAuthorizationClient Create()
    {
        var settings = options.Value;
        if (settings.ApiId <= 0 || !IsHex(settings.ApiHash, 32))
            throw new TelegramAuthorizationException("Telegram API credentials are not configured.",
                HttpStatusCode.ServiceUnavailable);

        return new TelegramAuthorizationClient(settings);
    }

    private static bool IsHex(string value, int length)
    {
        return value is not null && value.Length == length && value.All(Uri.IsHexDigit);
    }
}
