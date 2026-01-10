namespace Scribbly.Cubby.UnitTests.Store_Tests;

public class AdjustableTimeProvider(TimeSpan adjustment) : TimeProvider
{
    public TimeSpan Adjustment { get; set; } = adjustment;

    public void AddMilliseconds(int milliseconds) =>
        Adjustment = Adjustment.Add(TimeSpan.FromMilliseconds(2));

    /// <inheritdoc />
    public override DateTimeOffset GetUtcNow()
    {
        return DateTimeOffset.UtcNow.Add(Adjustment);
    }
}