namespace Scribbly.Cubby;

/// <summary>
/// Constant values and methods used to ensure the backing buffers are laid out correctly
/// [0-7 Expiration][8-11 Value Length][12-15 Flags][15 ^ Value]
/// </summary>
internal static class EntryLayout
{
    internal const int HeaderSize = 16;

    internal const int ValueLengthPosition = 8;
    
    internal const int FlagsPosition = 12;
    
    internal const int EncodingPosition = 14;
}