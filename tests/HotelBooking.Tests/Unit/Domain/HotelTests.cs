using HotelBooking.Core.Domain.Entities;
using HotelBooking.Core.Domain.Enums;
using Xunit;

namespace HotelBooking.Tests.Unit.Domain;

public sealed class HotelTests
{
    private static Hotel Create(int stars = 4, decimal latitude = 32m, decimal longitude = 35m, string? imageUrl = null) =>
        Hotel.Create(1, 1, " Grand ", " Nice. ", stars, HotelType.Luxury, " 1 Main St ", latitude, longitude, imageUrl);

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

    [Theory]
    [InlineData(" http://img/grand.png ", "http://img/grand.png")]
    [InlineData(null, null)]
    public void Create_ImageUrl_IsTrimmedOrNull(string? imageUrl, string? expected) =>
        Assert.Equal(expected, Create(imageUrl: imageUrl).HotelImageUrl);

    [Fact]
    public void Update_ReplacesTheImageUrl()
    {
        var hotel = Create(imageUrl: "http://img/old.png");

        hotel.Update(1, "Grand", "Nice.", 4, HotelType.Luxury, "1 Main St", 32m, 35m, " http://img/new.png ");

        Assert.Equal("http://img/new.png", hotel.HotelImageUrl);
    }

    /// <summary>The PUT replaces every field, so an update with no image means "remove it".</summary>
    [Fact]
    public void Update_WithoutAnImageUrl_ClearsIt()
    {
        var hotel = Create(imageUrl: "http://img/old.png");

        hotel.Update(1, "Grand", "Nice.", 4, HotelType.Luxury, "1 Main St", 32m, 35m);

        Assert.Null(hotel.HotelImageUrl);
    }
}