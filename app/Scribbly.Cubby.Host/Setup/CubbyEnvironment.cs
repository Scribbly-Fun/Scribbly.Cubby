using System.Diagnostics.CodeAnalysis;
using Scribbly.Cubby.Stores;

namespace Scribbly.Cubby.Host.Setup;

/// <summary>
/// Environment variable keys
/// </summary>
internal class CubbyEnvironment
{
    /// <summary>
    /// When set the application will be hosted with https.
    /// </summary>
    internal const string UseHttpsEnv = "SCRB_CUBBY_HTTPS";
}

[ExcludeFromCodeCoverage]
internal static partial class ApplicationLogger
{
    [LoggerMessage(EventId = 10_000, Level = LogLevel.Information, Message = 
        """
        Started Cubby With Options: 
            Store: {Store}
            Transports: {Transports}
            CPU Cores: {Cores}
            Capacity: {Capacity}
        """)]
    internal static partial void LogApplicationStartup(
        this ILogger logger,
        CubbyServerOptions.StoreType store,
        CubbyServerOptions.EnabledTransports transports,
        int cores,
        int capacity);
}