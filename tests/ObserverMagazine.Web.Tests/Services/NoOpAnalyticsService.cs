using ObserverMagazine.Web.Services;

namespace ObserverMagazine.Web.Tests.Services;

public sealed class NoOpAnalyticsService : IAnalyticsService
{
    public bool IsBackendAvailable => false;
    public Task CheckHealthAsync() => Task.CompletedTask;
    public Task TrackPageViewAsync(string pageName, string? detail = null) => Task.CompletedTask;
    public Task TrackInteractionAsync(string action, string? detail = null) => Task.CompletedTask;
    public Task IncrementViewAsync(string slug) => Task.CompletedTask;
    public Task<int?> GetViewCountAsync(string slug) => Task.FromResult<int?>(null);
    public Task AddReactionAsync(string slug, string reactionType) => Task.CompletedTask;
    public Task<Dictionary<string, int>?> GetReactionsAsync(string slug) =>
        Task.FromResult<Dictionary<string, int>?>(null);
}
