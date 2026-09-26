using HotelBooking.Core.Domain.ValueObjects;

namespace HotelBooking.Tests.Unit.Domain;

public sealed class DateRangeTests
{
    private static readonly DateOnly Monday = new(2026, 6, 1);

    private static DateRange Range(int fromDay, int toDay) =>
        DateRange.Create(Monday.AddDays(fromDay), Monday.AddDays(toDay));

    [Fact]
    public void Nights_CountsTheDaysBetween() =>
        Assert.Equal(3, Range(0, 3).Nights);

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Create_CheckOutNotAfterCheckIn_Throws(int checkOutOffset) =>
        Assert.Throws<ArgumentException>(() => Range(0, checkOutOffset));

    /// <summary>Two stays clash only when they share a night; back-to-back stays do not.</summary>
    [Theory]
    [InlineData(0, 3, 3, 5, false)]
    [InlineData(3, 5, 0, 3, false)]
    [InlineData(0, 3, 2, 5, true)]
    [InlineData(0, 5, 1, 2, true)]
    [InlineData(0, 3, 0, 3, true)]
    public void Overlaps_IsTrueOnlyWhenTheStaysShareANight(int fromA, int toA, int fromB, int toB, bool expected) =>
        Assert.Equal(expected, Range(fromA, toA).Overlaps(Range(fromB, toB)));

    /// <summary>Check-out day is not a night spent, so it is not contained.</summary>
    [Theory]
    [InlineData(0, true)]
    [InlineData(2, true)]
    [InlineData(3, false)]
    public void Contains_IncludesCheckInButNotCheckOut(int dayOffset, bool expected) =>
        Assert.Equal(expected, Range(0, 3).Contains(Monday.AddDays(dayOffset)));
}