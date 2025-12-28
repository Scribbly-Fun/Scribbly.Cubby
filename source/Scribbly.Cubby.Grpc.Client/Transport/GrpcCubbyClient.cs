using Scribbly.Cubby.Client;
using Scribbly.Cubby.Client.Serializer;

namespace Scribbly.Cubby;

/// <remarks>This is used when multiple transports are configured in a single application.</remarks>
internal sealed class GrpcCubbyClient(
    ICubbyStoreTransport store, 
    ICubbySerializer serializer) 
    : CubbyClient(store, serializer), IGrpcCubbyClient;