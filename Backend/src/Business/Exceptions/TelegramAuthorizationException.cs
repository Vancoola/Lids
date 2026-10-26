using System.Net;

namespace Business.Exceptions;

public sealed class TelegramAuthorizationException(
    string message,
    HttpStatusCode status = HttpStatusCode.BadRequest) : BusinessException(message)
{
    public override HttpStatusCode Status => status;
}
