using System.Buffers;
using System.Collections.Concurrent;

namespace Scribbly.Cubby.Stores.Sharded;

internal static class DictionaryExtensions
{
    extension(ConcurrentDictionary<BytesKey, byte[]> dictionary)
    {
        internal bool TryRemoveRentedArray(BytesKey key)
        {
            if (!dictionary.TryRemove(key, out var entry))
            {
                return false;
            }
            
            ArrayPool<byte>.Shared.Return(entry, clearArray: false);
            return true;
        }
    }
}