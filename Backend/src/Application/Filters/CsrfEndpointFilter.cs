using Microsoft.AspNetCore.Antiforgery;

namespace Application.Filters;

public sealed class CsrfEndpointFilter(
    IAntiforgery antiforgery) : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(
        EndpointFilterInvocationContext context,
        EndpointFilterDelegate next)
    {
        var http = context.HttpContext;

        if (HttpMethods.IsGet(http.Request.Method) ||
            HttpMethods.IsHead(http.Request.Method) ||
            HttpMethods.IsOptions(http.Request.Method))
        {
            return await next(context);
        }

        if (!await antiforgery.IsRequestValidAsync(http))
        {
            return Results.Problem(
                statusCode: 400,
                title: "Invalid CSRF token");
        }

        return await next(context);
    }
}