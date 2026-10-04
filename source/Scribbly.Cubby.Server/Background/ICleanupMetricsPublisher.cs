namespace Scribbly.Cubby.Server.Background;

/// <summary>
/// Publishes cleanup pass metrics from the async processor onto a consumer pipeline.
/// </summary>
internal interface ICleanupMetricsPublisher
{
    /// <summary>
    /// Attempts to publish a cleanup pass metric without blocking the cleanup hot path.
    /// </summary>
    /// <param name="metric">The metric captured for the completed pass.</param>
    void Publish(in CleanupPassMetric metric);
}
