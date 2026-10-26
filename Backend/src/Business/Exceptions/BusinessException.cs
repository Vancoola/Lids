using System.Net;

namespace Business.Exceptions;

public abstract class BusinessException(string message) : Exception(message)
{
    public abstract HttpStatusCode Status { get; }
}
