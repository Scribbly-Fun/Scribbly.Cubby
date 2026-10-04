using System.Collections.Immutable;
using System.Text.Json.Serialization;
using Scribbly.Cubby.Stores;

namespace Scribbly.Cubby.Host.Portal.Metrics;

[JsonSerializable(typeof(CleanupMetricsResponse))]
[JsonSerializable(typeof(CleanupMetricPoint))]
[JsonSerializable(typeof(ImmutableArray<CleanupMetricPoint>))]
internal partial class CleanupMetricsJsonContext : JsonSerializerContext;

public sealed record CleanupMetricsResponse(
    [property: JsonPropertyName("resolution")] string Resolution,
    [property: JsonPropertyName("sample_delay")] TimeSpan SampleDelay,
    [property: JsonPropertyName("capacity")] int Capacity,
    [property: JsonPropertyName("from")] DateTimeOffset From,
    [property: JsonPropertyName("to")] DateTimeOffset To,
    [property: JsonPropertyName("points")] ImmutableArray<CleanupMetricPoint> Points);

public sealed record CleanupMetricPoint(
    [property: JsonPropertyName("timestamp")] DateTimeOffset Timestamp,
    [property: JsonPropertyName("items_total")] int ItemsTotal,
    [property: JsonPropertyName("items_serviced")] int ItemsServiced,
    [property: JsonPropertyName("removed_tombstone")] int RemovedTombstone,
    [property: JsonPropertyName("removed_expired")] int RemovedExpired,
    [property: JsonPropertyName("removed_sliding")] int RemovedSliding,
    [property: JsonPropertyName("skipped_due_to_writers")] bool SkippedDueToWriters,
    [property: JsonPropertyName("hit_deadline")] bool HitDeadline,
    [property: JsonPropertyName("duration_ms")] double DurationMs);

internal static class CleanupMetricsMapping
{
    public static CleanupMetricsResponse ToResponse(
        CleanupMetricsStore store,
        TimeSpan fallbackDelay,
        DateTimeOffset? from = null,
        DateTimeOffset? to = null)
    {
        var windowTo = to ?? DateTimeOffset.UtcNow;
        var windowFrom = from ?? windowTo - TimeSpan.FromHours(1);

        if (windowFrom > windowTo)
        {
            (windowFrom, windowTo) = (windowTo, windowFrom);
        }

        var snapshot = store.Snapshot(windowFrom, windowTo);
        var sampleDelay = store.LatestSampleDelay ?? fallbackDelay;
        var points = snapshot.IsDefaultOrEmpty
            ? ImmutableArray<CleanupMetricPoint>.Empty
            : snapshot.Select(static m => new CleanupMetricPoint(
                m.Timestamp,
                m.ItemsTotal,
                m.ItemsServiced,
                m.RemovedTombstone,
                m.RemovedExpired,
                m.RemovedSliding,
                m.SkippedDueToWriters,
                m.HitDeadline,
                m.Duration.TotalMilliseconds)).ToImmutableArray();

        return new CleanupMetricsResponse(
            FormatResolution(sampleDelay),
            sampleDelay,
            store.Capacity,
            windowFrom,
            windowTo,
            points);
    }

    public static string FormatResolution(TimeSpan sampleDelay)
    {
        if (sampleDelay <= TimeSpan.Zero)
        {
            return "Sampled continuously";
        }

        var samplesPerHour = TimeSpan.FromHours(1) / sampleDelay;
        if (samplesPerHour >= 1)
        {
            return $"Sampled {samplesPerHour:0}x per hour";
        }

        var samplesPerDay = TimeSpan.FromDays(1) / sampleDelay;
        return $"Sampled {samplesPerDay:0}x per day";
    }

    public static TimeSpan ResolveFallbackDelay(CacheCleanupOptions cleanup) =>
        cleanup.Strategy switch
        {
            CacheCleanupOptions.AsyncStrategy.Hourly => TimeSpan.FromHours(1),
            CacheCleanupOptions.AsyncStrategy.Aggressive => TimeSpan.FromMilliseconds(250),
            CacheCleanupOptions.AsyncStrategy.Duration => cleanup.Delay,
            CacheCleanupOptions.AsyncStrategy.Random => TimeSpan.FromMinutes(20),
            _ => TimeSpan.Zero,
        };
}
