using Microsoft.Extensions.Logging;

namespace Scribbly.Cubby.Expiration;

internal static partial class ExpirationEvictionLogger
{
    [LoggerMessage(
        EventId = 10_101, 
        Level = LogLevel.Trace, 
        Message = "Determined Lock Contention, Skipping Cleanup")]
    internal static partial void LogCleanupSkipped(
        this ILogger logger);

    [LoggerMessage(
        EventId = 10_102, 
        Level = LogLevel.Trace, 
        Message = "Determined Deadline Reached, Stopping Cleanup; Iterations: {Iterations} Deadline: {Deadline}")]
    internal static partial void LogCleanupDeadline(
        this ILogger logger,
        int iterations,
        long deadline);

    [LoggerMessage(
        EventId = 10_103, 
        Level = LogLevel.Trace, 
        Message = "Detected Expired Cache {Key}, Removing")]
    internal static partial void LogEntryCleared(
        this ILogger logger,
        BytesKey key);
}