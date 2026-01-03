using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace Scribbly.Cubby.Stores.LockFree;

[Experimental("SCRB001", Message = "Not yet implemented")]
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