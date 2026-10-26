using System.Security.Claims;
using System.Threading.RateLimiting;
using Application.Workers;
using Business.Abstractions.Telegram;
using Business.Telegram;
using Data.Telegram;
using Infrastructure.Telegram;

namespace Application.Extensions;

public static class TelegramExtensions
{
    public static IServiceCollection AddTelegramServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<TelegramOptions>(configuration.GetSection("Telegram"));
        services.AddSingleton(TimeProvider.System);
        services.AddSingleton<ITelegramAuthorizationClientFactory, TelegramAuthorizationClientFactory>();
        services.AddSingleton<ITelegramAuthorizationService, TelegramAuthorizationService>();
        services.AddSingleton<ITelegramAgentStore, TelegramAgentStore>();
        services.AddHostedService<TelegramAuthorizationCleanupWorker>();

        services.AddRateLimiter(options =>
        {
            options.AddPolicy("telegram-authorization", context =>
                RateLimitPartition.GetFixedWindowLimiter(
                    context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ??
                    context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                    _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 30,
                        Window = TimeSpan.FromMinutes(1),
                        QueueLimit = 0,
                        AutoReplenishment = true
                    }));
        });

        return services;
    }
}