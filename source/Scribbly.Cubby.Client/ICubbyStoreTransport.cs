using Scribbly.Cubby.Stores;

namespace Scribbly.Cubby.Client;

/// <summary>
/// A transport interface communicates directly with the cubby server using the configured transport.
/// Transports will consist of
///     1. GRPC client / server
///     2. HTTP client / server
///     3. TCP socket client / server
/// </summary>
internal interface ICubbyStoreTransport : IAsyncCubbyStoreTransport, ISyncCubbyStoreTransport;

/// <summary>
/// A transport interface communicates directly with the cubby server using the configured transport.
/// Transports will consist of
///     1. GRPC client / server
///     2. HTTP client / server
///     3. TCP socket client / server
/// </summary>
internal interface IAsyncCubbyStoreTransport
{
    ValueTask<PutResult> PutAsync(BytesKey key, ReadOnlyMemory<byte> value, CacheEntryOptions? options = null, CancellationToken token = default);
    
    ValueTask<ReadOnlyMemory<byte>> GetAsync(BytesKey key, CancellationToken token = default);
    
    ValueTask<bool> ExistsAsync(BytesKey key, CancellationToken token = default);
    
    ValueTask<RefreshResult> RefreshAsync(BytesKey key, CancellationToken token = default);
    
    ValueTask<EvictResult> EvictAsync(BytesKey key, CancellationToken token = default);
}

/// <summary>
/// A transport interface communicates directly with the cubby server using the configured transport.
/// Transports will consist of
///     1. GRPC client / server
///     2. HTTP client / server
///     3. TCP socket client / server
/// </summary>
internal interface ISyncCubbyStoreTransport
{
    PutResult Put(in BytesKey key, ReadOnlyMemory<byte> value, CacheEntryOptions? options = null);
    
    ReadOnlyMemory<byte> Get(in BytesKey key);
    
    bool Exists(in BytesKey key);
    
    RefreshResult Refresh(in BytesKey key);
    
    EvictResult Evict(in BytesKey key);
}