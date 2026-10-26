using System.Net;

namespace Business.Exceptions;

public sealed class TelegramAccountAlreadyConnectedException()
    : BusinessException("This Telegram account is already connected.")
{
    public override HttpStatusCode Status => HttpStatusCode.Conflict;
}