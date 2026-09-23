using HotelBooking.Core.Domain.Entities;
using HotelBooking.Core.Domain.Enums;
using Xunit;

namespace HotelBooking.Tests.Unit.Domain;

public sealed class HotelTests
{
    private static Hotel Create(int stars = 4, decimal latitude = 32m, decimal longitude = 35m) =>
        Hotel.Create(1, 1, " Grand ", " Nice. ", stars, HotelType.Luxury, " 1 Main St ", latitude, longitude);

    [Fact]
    public void Create_TrimsTheText() =>
        Assert.Equal("Grand", Create().Name);

    [Theory]
    [InlineData(0)]
    [InlineData(6)]
    public void Create_StarsOutsideOneToFive_Throws(int stars) =>
        Assert.Throws<ArgumentOutOfRangeException>(() => Create(stars: stars));

    [Theory]
    [InlineData(-90.000001)]
    [InlineData(90.000001)]
    public void Create_LatitudeOffTheGlobe_Throws(double latitude) =>
        Assert.Throws<ArgumentOutOfRangeException>(() => Create(latitude: (decimal)latitude));

    [Fact]
    public void Create_LongitudeAtTheDateLine_IsAllowed() =>
        Assert.Equal(180m, Create(longitude: 180m).Longitude);
}