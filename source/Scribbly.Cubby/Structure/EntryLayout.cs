namespace Scribbly.Cubby;

/// <summary>
/// Constant values and methods used to ensure the backing buffers are laid out correctly
/// [0-7 Expiration][8-11 SlidingDuration][12-15 Value Length][16-24 Flags][15 ^ Value]
/// </summary>
internal static class EntryLayout
{
    internal const int HeaderSize = 24;

    internal const int ExpirationPosition = 0;
    
    internal const int DurationPosition = 8;
    
    internal const int ValueLengthPosition = 16;
    
    internal const int FlagsPosition = 20;
    
    internal const int EncodingPosition = 22;
}