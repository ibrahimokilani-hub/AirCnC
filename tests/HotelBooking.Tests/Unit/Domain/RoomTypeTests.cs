using HotelBooking.Core.Domain.Entities;

namespace HotelBooking.Tests.Unit.Domain;

public sealed class RoomTypeTests
{
    private static RoomType Standard() => RoomType.Create(1, "Standard", "", 100m, 2, 1);

    [Theory]
    [InlineData(2, 1, true)]
    [InlineData(1, 0, true)]
    [InlineData(3, 0, false)]
    [InlineData(2, 2, false)]
    public void Fits_ComparesAdultsAndChildrenSeparately(int adults, int children, bool expected) =>
        Assert.Equal(expected, Standard().Fits(adults, children));

    [Fact]
    public void Create_ZeroPrice_Throws() =>
        Assert.Throws<ArgumentOutOfRangeException>(() => RoomType.Create(1, "Free", "", 0m, 2, 0));

    [Fact]
    public void Create_NoAdults_Throws() =>
        Assert.Throws<ArgumentOutOfRangeException>(() => RoomType.Create(1, "Empty", "", 50m, 0, 0));
}