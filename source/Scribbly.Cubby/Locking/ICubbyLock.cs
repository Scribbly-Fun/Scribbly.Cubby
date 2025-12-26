namespace Scribbly.Cubby.Locking;

/// <summary>
///     Obtains a distrubuted lock for the Cubby server using the selected transpoint.
/// </summary>
public interface ICubbyLock
{
    /// <summary>
    ///     Requests a lock for the key provided.
    ///     Requesting a lock will block the current thread when the lock is already obtained by another client.
    /// </summary>
    /// <param name="key">
    ///     The locks key
    /// </param>
    /// <param name="timeout">
    ///     An optional timeout. Should the lock fail to obtain withing the provided timeout a <see cref="TimeoutException"/> will be thrown.
    /// </param>
    /// <param name="token">
    ///     Cancellation token used to stop the lock request.
    /// </param>
    /// <returns>
    ///     An Async Process
    /// </returns>
    Task Lock(BytesKey key, TimeSpan timeout = default, CancellationToken token = default);
}