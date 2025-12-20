using System.Runtime.InteropServices;

namespace Scribbly.Cubby.Stores.LockFree;

[StructLayout(LayoutKind.Sequential)]
internal struct Entry
{
    public int Hash;
    public long ExpirationTicks;
    public byte[] Key;
    public byte[] Value;
    
    /// <summary>
    /// 0=empty, 1=used, 2=deleted
    /// </summary>
    public int State;
}