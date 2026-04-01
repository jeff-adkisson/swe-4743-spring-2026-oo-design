namespace backend.Services;

public sealed class StaleSessionCleanupService : BackgroundService
{
    private static readonly TimeSpan SweepInterval = TimeSpan.FromSeconds(15);
    private static readonly TimeSpan MaxIdle = TimeSpan.FromSeconds(35);

    private readonly ChatCoordinator _coordinator;
    private readonly ILogger<StaleSessionCleanupService> _logger;

    public StaleSessionCleanupService(
        ChatCoordinator coordinator,
        ILogger<StaleSessionCleanupService> logger)
    {
        _coordinator = coordinator;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var removed = await _coordinator.RemoveStaleSessionsAsync(MaxIdle, stoppingToken);

                if (removed > 0)
                {
                    _logger.LogInformation("Removed {Count} stale chat session(s).", removed);
                }
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Stale chat session cleanup failed.");
            }

            await Task.Delay(SweepInterval, stoppingToken);
        }
    }
}
