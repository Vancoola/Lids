using System.Net;

namespace Business.Exceptions;

public sealed class TelegramAuthorizationNotFoundException()
    : BusinessException("Telegram authorization attempt was not found or has expired.")
{
    public override HttpStatusCode Status => HttpStatusCode.NotFound;
}