using Microsoft.Extensions.Logging;

namespace ObserverMagazine.Web.Services;

/// <summary>
/// Lightweight telemetry service for tracking events and metrics in the browser.
/// Logs to ILogger (browser console). Can be extended with a real OpenTelemetry
/// collector endpoint when one is available.
/// </summary>
public sealed class TelemetryService(ILogger<TelemetryService> logger)
{
    private readonly Dictionary<string, long> counters = new();
    private readonly object lockObj = new();

    public void TrackEvent(string eventName, IDictionary<string, string>? properties = null)
    {
        var props = properties is not null
            ? string.Join(", ", properties.Select(kv => $"{kv.Key}={kv.Value}"))
            : "";
        logger.LogInformation("[Telemetry] Event: {EventName} {Properties}", eventName, props);
    }

    public void IncrementCounter(string counterName, long value = 1)
    {
        lock (lockObj)
        {
            counters.TryGetValue(counterName, out var current);
            counters[counterName] = current + value;
        }
        logger.LogDebug("[Telemetry] Counter: {CounterName} += {Value}", counterName, value);
    }

    public long GetCounter(string counterName)
    {
        lock (lockObj)
        {
            return counters.GetValueOrDefault(counterName, 0);
        }
    }

    public void TrackPageView(string pageName)
    {
        IncrementCounter($"pageview:{pageName}");
        TrackEvent("PageView", new Dictionary<string, string> { ["page"] = pageName });
    }
}
