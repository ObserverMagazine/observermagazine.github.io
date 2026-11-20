using Microsoft.Extensions.Hosting;

namespace ObserverMagazine.Web.Services;

public sealed class AppLifetimeTelemetryService(
    TelemetryService telemetry,
    ILogger<AppLifetimeTelemetryService> logger) : IHostedService
{
    public const string StartedEvent = "AppStarted";
    public const string StoppedEvent = "AppStopped";
    public const string StartCounter = "app.starts";

    public Task StartAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Observer Magazine started at {Time}", DateTime.UtcNow);
        telemetry.IncrementCounter(StartCounter);
        telemetry.TrackEvent(StartedEvent);
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        telemetry.TrackEvent(StoppedEvent);
        return Task.CompletedTask;
    }
}
