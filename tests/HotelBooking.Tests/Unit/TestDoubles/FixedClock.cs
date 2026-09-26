namespace HotelBooking.Tests.Unit.TestDoubles;

/// <summary>
/// A clock frozen at one instant. Validators that compare against "today" need a fixed
/// today, or they would pass in June and fail in July.
/// </summary>
public sealed class FixedClock(DateTimeOffset utcNow) : TimeProvider
{
    public static readonly DateOnly Today = new(2026, 6, 10);

    public static FixedClock AtToday() =>
        new(new DateTimeOffset(Today.Year, Today.Month, Today.Day, 9, 0, 0, TimeSpan.Zero));

    public override DateTimeOffset GetUtcNow() => utcNow;
}