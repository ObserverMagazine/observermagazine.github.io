namespace ObserverMagazine.Web.Services;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddObserverMagazine(this IServiceCollection services, Uri baseAddress)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(baseAddress);

        services.AddScoped(_ => new HttpClient { BaseAddress = baseAddress });
        services.AddScoped<IBlogService, BlogService>();
        services.AddScoped<IAnalyticsService, AnalyticsService>();
        services.AddSingleton<TelemetryService>();
        services.AddHostedService<AppLifetimeTelemetryService>();

        return services;
    }
}