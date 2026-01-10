namespace Scribbly.Cubby.UnitTests.Setup;

public class StaticTimeProvider : TimeProvider
{
    /// <inheritdoc />
    public override DateTimeOffset GetUtcNow()
    {
        return new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.FromSeconds(0));
    }
}