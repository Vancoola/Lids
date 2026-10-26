using Business.Exceptions;
using Microsoft.AspNetCore.Diagnostics;

namespace Application.Exceptions;

public sealed class BusinessExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken ct)
    {
        if (exception is not BusinessException businessException)
            return false;

        await Results.Problem(
            statusCode: (int)businessException.Status,
            title: "Request failed",
            detail: businessException.Message).ExecuteAsync(context);

        return true;
    }
}