using System.Collections.Immutable;
using Scribbly.Cubby.Server.Background;

namespace Scribbly.Cubby.Host.Portal.Metrics;

/// <summary>
/// Bounded in-memory ring of cleanup metrics available for portal queries.
/// </summary>
internal sealed class CleanupMetricsStore
{
    public const int DefaultCapacity = 1_440;

    private readonly Lock _lock = new();
    private readonly CleanupPassMetric[] _buffer;
    private int _count;
    private int _next;

    public CleanupMetricsStore(int capacity = DefaultCapacity)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(capacity, 1);
        _buffer = new CleanupPassMetric[capacity];
    }

    public int Capacity => _buffer.Length;

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

    public ImmutableArray<CleanupPassMetric> Snapshot()
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
                builder.Add(_buffer[(start + i) % _buffer.Length]);
            }

            return builder.MoveToImmutable();
        }
    }

    private int PreviousIndex(int index) =>
        index == 0 ? _buffer.Length - 1 : index - 1;
}
