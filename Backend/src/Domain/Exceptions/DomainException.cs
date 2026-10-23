using System.Net;

namespace Domain.Exceptions;

public abstract class DomainException(string message) : Exception(message)
{
    public abstract HttpStatusCode Status { get; }
}