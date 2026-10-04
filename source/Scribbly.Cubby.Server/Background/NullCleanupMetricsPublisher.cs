namespace Scribbly.Cubby.Server.Background;

/// <summary>
/// No-op publisher used when the host does not register a metrics consumer.
/// </summary>
internal sealed class NullCleanupMetricsPublisher : ICleanupMetricsPublisher
{
    public static readonly NullCleanupMetricsPublisher Instance = new();

    public void Publish(in CleanupPassMetric metric)
    {
    }
}
