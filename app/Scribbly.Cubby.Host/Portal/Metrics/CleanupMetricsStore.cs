using System.Collections.Immutable;
using Microsoft.Extensions.Options;
using Scribbly.Cubby.Server.Background;
using Scribbly.Cubby.Stores;

namespace Scribbly.Cubby.Host.Portal.Metrics;

/// <summary>
/// Bounded in-memory ring of cleanup metrics available for portal queries.
/// Capacity is derived from <see cref="IOptionsMonitor{CubbyServerOptions}"/> and resized when cleanup delay changes.
/// </summary>
internal sealed class CleanupMetricsStore
{
    private readonly Lock _lock = new();
    private readonly ILogger<CleanupMetricsStore> _logger;
    private CleanupPassMetric[] _buffer;
    private int _count;
    private int _next;

    public CleanupMetricsStore(
        IOptionsMonitor<CubbyServerOptions> optionsMonitor,
        ILogger<CleanupMetricsStore> logger)
    {
        _logger = logger;
        var capacity = CleanupMetricsCapacity.Compute(optionsMonitor.CurrentValue.Cleanup);
        _buffer = new CleanupPassMetric[capacity];

        logger.LogInformation(
            "Cleanup metrics store initialized with capacity {Capacity} for delay strategy {Strategy}",
            capacity,
            optionsMonitor.CurrentValue.Cleanup.Strategy);

        optionsMonitor.OnChange(options =>
        {
            var nextCapacity = CleanupMetricsCapacity.Compute(options.Cleanup);
            Resize(nextCapacity, options.Cleanup);
        });
    }

    public int Capacity
    {
        get
        {
            lock (_lock)
            {
                return _buffer.Length;
            }
        }
    }

    public TimeSpan? LatestSampleDelay
    {
        get
        {
            lock (_lock)
            {
                return _count == 0 ? null : _buffer[PreviousIndex(_next)].SampleDelay;
            }
        }
    }

    public void Push(in CleanupPassMetric metric)
    {
        lock (_lock)
        {
            _buffer[_next] = metric;
            _next = (_next + 1) % _buffer.Length;
            if (_count < _buffer.Length)
            {
                _count++;
            }
        }
    }

    public ImmutableArray<CleanupPassMetric> Snapshot(DateTimeOffset? from = null, DateTimeOffset? to = null)
    {
        lock (_lock)
        {
            if (_count == 0)
            {
                return [];
            }

            var builder = ImmutableArray.CreateBuilder<CleanupPassMetric>(_count);
            var start = _count < _buffer.Length ? 0 : _next;

            for (var i = 0; i < _count; i++)
            {
                var metric = _buffer[(start + i) % _buffer.Length];

                if (from is { } fromValue && metric.Timestamp < fromValue)
                {
                    continue;
                }

                if (to is { } toValue && metric.Timestamp > toValue)
                {
                    continue;
                }

                builder.Add(metric);
            }

            return builder.Count == 0 ? [] : builder.ToImmutable();
        }
    }

    private void Resize(int newCapacity, CacheCleanupOptions cleanup)
    {
        lock (_lock)
        {
            if (newCapacity == _buffer.Length)
            {
                return;
            }

            var previousCapacity = _buffer.Length;
            var ordered = CopyOrderedUnsafe();
            var keep = Math.Min(ordered.Length, newCapacity);
            var nextBuffer = new CleanupPassMetric[newCapacity];

            if (keep > 0)
            {
                var sourceStart = ordered.Length - keep;
                Array.Copy(ordered, sourceStart, nextBuffer, 0, keep);
            }

            _buffer = nextBuffer;
            _count = keep;
            _next = keep == newCapacity ? 0 : keep;

            _logger.LogInformation(
                "Cleanup metrics capacity resized from {PreviousCapacity} to {Capacity} for strategy {Strategy} delay {Delay}",
                previousCapacity,
                newCapacity,
                cleanup.Strategy,
                CleanupMetricsCapacity.ResolveMinimumSampleInterval(cleanup));
        }
    }

    private CleanupPassMetric[] CopyOrderedUnsafe()
    {
        if (_count == 0)
        {
            return [];
        }

        var ordered = new CleanupPassMetric[_count];
        var start = _count < _buffer.Length ? 0 : _next;

        for (var i = 0; i < _count; i++)
        {
            ordered[i] = _buffer[(start + i) % _buffer.Length];
        }

        return ordered;
    }

    private int PreviousIndex(int index) =>
        index == 0 ? _buffer.Length - 1 : index - 1;
}
