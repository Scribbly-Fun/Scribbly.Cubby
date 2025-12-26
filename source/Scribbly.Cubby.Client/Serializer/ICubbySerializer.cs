namespace Scribbly.Cubby.Client.Serializer;

/// <summary>
/// Abstract used to serialize data in and out of the cache.
/// </summary>
public interface ICubbySerializer
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    ReadOnlySpan<byte> Serialize<T>(T value);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="data"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    T? Deserialize<T>(ReadOnlySpan<byte> data);
}