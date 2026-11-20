using Microsoft.Extensions.Logging.Abstractions;
using ObserverMagazine.Web.Services;
using Xunit;

namespace ObserverMagazine.Web.Tests.Services;

public class AppLifetimeTelemetryServiceTests
{
    private static (AppLifetimeTelemetryService Service, TelemetryService Telemetry) CreateSut()
    {
        var telemetry = new TelemetryService(NullLogger<TelemetryService>.Instance);
        var service = new AppLifetimeTelemetryService(telemetry, NullLogger<AppLifetimeTelemetryService>.Instance);
        return (service, telemetry);
    }

    [Fact]
    public async Task StartAsync_IncrementsStartCounter()
    {
        var (service, telemetry) = CreateSut();

        await service.StartAsync(TestContext.Current.CancellationToken);

        Assert.Equal(1L, telemetry.GetCounter(AppLifetimeTelemetryService.StartCounter));
    }

    [Fact]
    public async Task StartAsync_CalledTwice_CountsEachStart()
    {
        var (service, telemetry) = CreateSut();

        await service.StartAsync(TestContext.Current.CancellationToken);
        await service.StartAsync(TestContext.Current.CancellationToken);

        Assert.Equal(2L, telemetry.GetCounter(AppLifetimeTelemetryService.StartCounter));
    }

    [Fact]
    public async Task StopAsync_DoesNotChangeStartCounter()
    {
        var (service, telemetry) = CreateSut();

        await service.StartAsync(TestContext.Current.CancellationToken);
        await service.StopAsync(TestContext.Current.CancellationToken);

        Assert.Equal(1L, telemetry.GetCounter(AppLifetimeTelemetryService.StartCounter));
    }

    [Fact]
    public async Task StartAsync_CompletesSynchronously()
    {
        var (service, _) = CreateSut();

        var task = service.StartAsync(TestContext.Current.CancellationToken);

        Assert.True(task.IsCompletedSuccessfully);
        await task;
    }
}
