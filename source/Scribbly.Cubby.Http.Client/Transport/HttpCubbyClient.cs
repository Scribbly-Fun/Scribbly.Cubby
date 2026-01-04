using Scribbly.Cubby.Client;
using Scribbly.Cubby.Client.Serializer;

namespace Scribbly.Cubby;

/// <remarks>This is used when multiple transports are configured in a single application.</remarks>
internal sealed class HttpCubbyClient(
    IHttpCubbyStoreTransport store, 
    ICubbySerializer serializer,
    ICubbyCompressor compressor) 
    : CubbyClient(store, serializer, compressor), IHttpCubbyClient;