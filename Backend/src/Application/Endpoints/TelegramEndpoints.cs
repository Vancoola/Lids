using System.Net;
using System.Security.Claims;
using Application.Filters;
using Business.Abstractions.Telegram;
using Business.Contracts.Telegram;
using Business.Exceptions;

namespace Application.Endpoints;

public static class TelegramEndpoints
{
    public static IEndpointRouteBuilder MapTelegramEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/agents/telegram/authorizations")
            .WithTags("Telegram authorization")
            .RequireAuthorization(policy => policy.RequireRole("SuperAdmin"))
            .RequireRateLimiting("telegram-authorization")
            .AddEndpointFilter<CsrfEndpointFilter>()
            .AddEndpointFilter(async (context, next) =>
            {
                context.HttpContext.Response.Headers.CacheControl = "no-store";
                return await next(context);
            });

        group.MapPost("/", Start)
            .WithSummary("Начать подключение Telegram-аккаунта");

        group.MapGet("/{attemptId:guid}", Get)
            .WithSummary("Получить текущий шаг авторизации");

        group.MapPost("/{attemptId:guid}/continue", Continue)
            .WithSummary("Передать код, пароль 2FA или подтверждение email");

        group.MapPost("/{attemptId:guid}/complete", Complete)
            .WithSummary("Повторить сохранение авторизованного аккаунта");

        group.MapDelete("/{attemptId:guid}", Cancel)
            .WithSummary("Закрыть попытку авторизации");

        return app;
    }

    private static async Task<IResult> Start(
        StartTelegramAuthorizationRequest request,
        HttpContext context,
        ITelegramAuthorizationService service,
        CancellationToken ct)
    {
        var result = await service.StartAsync(OwnerId(context.User), request.PhoneNumber, ct);
        return Results.Ok(result);
    }

    private static async Task<IResult> Get(
        Guid attemptId,
        HttpContext context,
        ITelegramAuthorizationService service,
        CancellationToken ct)
    {
        var result = await service.GetAsync(OwnerId(context.User), attemptId, ct);
        return Results.Ok(result);
    }

    private static async Task<IResult> Continue(
        Guid attemptId,
        ContinueTelegramAuthorizationRequest request,
        HttpContext context,
        ITelegramAuthorizationService service,
        CancellationToken ct)
    {
        var result = await service.ContinueAsync(OwnerId(context.User), attemptId, request.Step, request.Value, ct);
        return Results.Ok(result);
    }

    private static async Task<IResult> Complete(
        Guid attemptId,
        HttpContext context,
        ITelegramAuthorizationService service,
        CancellationToken ct)
    {
        var result = await service.CompleteAsync(OwnerId(context.User), attemptId, ct);
        return Results.Ok(result);
    }

    private static async Task<IResult> Cancel(
        Guid attemptId,
        HttpContext context,
        ITelegramAuthorizationService service,
        CancellationToken ct)
    {
        await service.CancelAsync(OwnerId(context.User), attemptId, ct);
        return Results.NoContent();
    }

    private static Guid OwnerId(ClaimsPrincipal user)
    {
        if (!Guid.TryParse(user.FindFirstValue(ClaimTypes.NameIdentifier), out var ownerId))
            throw new TelegramAuthorizationException("Authentication is required.", HttpStatusCode.Unauthorized);

        return ownerId;
    }
}

public record StartTelegramAuthorizationRequest(string PhoneNumber);
public record ContinueTelegramAuthorizationRequest(TelegramAuthorizationStep Step, string Value);
