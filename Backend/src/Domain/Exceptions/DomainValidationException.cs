using System.Net;

namespace Domain.Exceptions;

public sealed class DomainValidationException(string message) : DomainException(message)
{
    public override HttpStatusCode Status => HttpStatusCode.BadRequest;
}