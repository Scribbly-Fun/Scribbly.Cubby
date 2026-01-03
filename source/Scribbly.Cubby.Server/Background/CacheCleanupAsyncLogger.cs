using Microsoft.Extensions.Logging;
using Scribbly.Cubby.Stores;

namespace Scribbly.Cubby.Server.Background;

/// <summary>
/// Logging methods for the background processor.
/// </summary>
internal static partial class CacheCleanupAsyncLogger
{
    [LoggerMessage(EventId = 10_001, Level = LogLevel.Information, Message = "Starting Cache Cleanup Background Processor, Store: {Store} Delay: {Delay}")]
    internal static partial void LogStartupMessage(
        this ILogger logger,
        CubbyOptions.StoreType store,
        TimeSpan delay);

    [LoggerMessage(EventId = 10_002, Level = LogLevel.Warning, Message = "Cubby Options Updated, New Delay: {Delay}")]
    internal static partial void LogOptionsUpdated(
        this ILogger logger,
        TimeSpan delay);

    [LoggerMessage(EventId = 10_003, Level = LogLevel.Trace, Message = "Executing Cache Cleanup Background Processor, Delay: {IntervalDelay} Execution Time: {ExecutionTime}")]
    internal static partial void LogIterationMessage(
        this ILogger logger,
        TimeSpan intervalDelay,
        TimeSpan executionTime);
}