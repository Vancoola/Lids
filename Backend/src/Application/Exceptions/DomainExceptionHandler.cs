using Domain.Exceptions;
using Microsoft.AspNetCore.Diagnostics;

namespace Application.Exceptions;

public sealed class DomainExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken ct)
    {
        if (exception is not DomainException domainException)
            return false;

        await Results.Problem(
            statusCode: (int)domainException.Status,
            title: "Request failed",
            detail: domainException.Message).ExecuteAsync(context);

        return true;
    }
}
