using Scribbly.Cubby.Stores;

namespace Scribbly.Cubby.Client;

/// <summary>
/// A transport interface communicates directly with the cubby server using the configured transport.
/// Transports will consist of
///     1. GRPC client / server
///     2. HTTP client / server
///     3. TCP socket client / server
/// </summary>
internal interface ICubbyStoreTransport
{
    ValueTask<bool> Exists(BytesKey key, CancellationToken token = default);
    
    ValueTask<PutResult> Put(BytesKey key, ReadOnlyMemory<byte> value, CacheEntryOptions options, CancellationToken token = default);
    
    ValueTask<ReadOnlyMemory<byte>> Get(BytesKey key, CancellationToken token = default);
}