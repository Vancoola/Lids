using Application.Filters;
using Data.Identity;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Identity;

namespace Application.Endpoints;

public static class AuthEndpoints
{
    public static IEndpointRouteBuilder MapAuthEndpoints(
        this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/auth")
            .WithTags("Authentication")
            .AddEndpointFilter<CsrfEndpointFilter>();

        group.MapGet("/csrf", (
                HttpContext context,
                IAntiforgery antiforgery) =>
            {
                var tokens = antiforgery.GetAndStoreTokens(context);

                return Results.Ok(new
                {
                    token = tokens.RequestToken
                });
            })
            .AllowAnonymous()
            .WithSummary("Получить CSRF-токен");

        group.MapPost("/login", Login)
            .AllowAnonymous()
            .WithSummary("Авторизация пользователя");

        group.MapPost("/logout", Logout)
            .RequireAuthorization()
            .WithSummary("Выход из аккаунта");

        group.MapGet("/me", Me)
            .RequireAuthorization()
            .WithSummary("Текущий пользователь");

        return app;
    }

    private static async Task<IResult> Login(
        LoginRequest request,
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager)
    {
        var user = await userManager.FindByEmailAsync(request.Email);

        if (user is null)
            return Results.Unauthorized();

        var result = await signInManager.PasswordSignInAsync(
            user,
            request.Password,
            false,
            true);

        return result.Succeeded
            ? Results.NoContent()
            : Results.Unauthorized();
    }

    private static async Task<IResult> Logout(
        SignInManager<ApplicationUser> signInManager)
    {
        await signInManager.SignOutAsync();

        return Results.NoContent();
    }

    private static async Task<IResult> Me(
        HttpContext context,
        UserManager<ApplicationUser> userManager)
    {
        var user = await userManager.GetUserAsync(context.User);

        if (user is null)
            return Results.Unauthorized();

        return Results.Ok(new
        {
            user.Id,
            user.Email,
            Roles = await userManager.GetRolesAsync(user)
        });
    }
}

public record LoginRequest(string Email, string Password);