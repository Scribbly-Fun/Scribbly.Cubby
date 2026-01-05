namespace Scribbly.Cubby.Stores;

/// <summary>
/// When supported by the store this interface allows a caller to iterate over the entire store.
/// </summary>
internal interface ICubbyStoreIterator
{
    /// <summary>
    /// Iterates over all entries in the store
    /// </summary>
    IEnumerable<KeyValuePair<BytesKey, byte[]>> Entries { get; } 
}