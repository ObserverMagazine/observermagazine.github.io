using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ObserverMagazine.Web.Services;
using Xunit;

namespace ObserverMagazine.Web.Tests.Services;

public class ServiceCollectionExtensionsTests
{
    private static readonly Uri BaseAddress = new("https://test.local/");

    private static ServiceProvider BuildProvider()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddObserverMagazine(BaseAddress);
        return services.BuildServiceProvider(new ServiceProviderOptions
        {
            ValidateScopes = true,
            ValidateOnBuild = true
        });
    }

    [Fact]
    public void AddObserverMagazine_RegistersAppLifetimeTelemetryAsHostedService()
    {
        using var provider = BuildProvider();

        var hosted = provider.GetServices<IHostedService>();

        Assert.IsType<AppLifetimeTelemetryService>(Assert.Single(hosted));
    }

    [Fact]
    public void AddObserverMagazine_ResolvesScopedServicesWithinScope()
    {
        using var provider = BuildProvider();
        using var scope = provider.CreateScope();

        Assert.IsType<BlogService>(scope.ServiceProvider.GetRequiredService<IBlogService>());
        Assert.IsType<AnalyticsService>(scope.ServiceProvider.GetRequiredService<IAnalyticsService>());
        Assert.Equal(BaseAddress, scope.ServiceProvider.GetRequiredService<HttpClient>().BaseAddress);
    }

    [Fact]
    public void AddObserverMagazine_TelemetryServiceIsSingleton()
    {
        using var provider = BuildProvider();
        using var scopeA = provider.CreateScope();
        using var scopeB = provider.CreateScope();

        Assert.Same(
            scopeA.ServiceProvider.GetRequiredService<TelemetryService>(),
            scopeB.ServiceProvider.GetRequiredService<TelemetryService>());
    }

    [Fact]
    public void AddObserverMagazine_ThrowsOnNullArguments()
    {
        Assert.Throws<ArgumentNullException>(() => ServiceCollectionExtensions.AddObserverMagazine(null!, BaseAddress));
        Assert.Throws<ArgumentNullException>(() => new ServiceCollection().AddObserverMagazine(null!));
    }
}
