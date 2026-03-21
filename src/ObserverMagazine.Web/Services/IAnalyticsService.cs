namespace ObserverMagazine.Web.Services;

/// <summary>
/// Sends analytics events to the Cloudflare Workers backend.
/// Gracefully degrades if the backend is unavailable.
/// </summary>
public interface IAnalyticsService
{
    Task TrackPageViewAsync(string pageName, string? detail = null);
    Task TrackInteractionAsync(string action, string? detail = null);
    bool IsBackendAvailable { get; }
    Task CheckHealthAsync();

    // --- View counts (backend-enhanced, optional) ---
    Task IncrementViewAsync(string slug);
    Task<int?> GetViewCountAsync(string slug);

    // --- Reactions (backend-enhanced, optional) ---
    Task AddReactionAsync(string slug, string reactionType);
    Task<Dictionary<string, int>?> GetReactionsAsync(string slug);
}
