using Scribbly.Cubby.Client;

namespace Scribbly.Cubby;

/// <summary>
/// An explicate client used to resolve the correct gRPC cubby client
/// </summary>
/// <remarks>This is used when multiple transports are configured in a single application.</remarks>
public interface IGrpcCubbyClient : ICubbyClient;