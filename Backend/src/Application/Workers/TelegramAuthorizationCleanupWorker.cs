using Business.Abstractions.Telegram;

namespace Application.Workers;

public sealed class TelegramAuthorizationCleanupWorker(
    ITelegramAuthorizationService authorizationService,
    ILogger<TelegramAuthorizationCleanupWorker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromMinutes(1));
        while (await timer.WaitForNextTickAsync(stoppingToken))
            try
            {
                await authorizationService.RemoveExpiredAsync(stoppingToken);
            }
            catch (Exception exception) when (exception is not OperationCanceledException)
            {
                logger.LogError("Failed to clean up expired Telegram authorizations: {ErrorType}",
                    exception.GetType().Name);
            }
    }
}