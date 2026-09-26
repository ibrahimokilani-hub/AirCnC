using HotelBooking.Core.Domain.Entities;

namespace HotelBooking.Tests.Unit.Domain;

public sealed class NearbyAttractionTests
{
    [Theory]
    [InlineData(" http://img/old-city.png ", "http://img/old-city.png")]
    [InlineData(null, null)]
    public void Create_ImageUrl_IsTrimmedOrNull(string? imageUrl, string? expected) =>
        Assert.Equal(expected, NearbyAttraction.Create("Old City", "Landmark", 32m, 35m, imageUrl).AttractionImageUrl);

    [Fact]
    public void Update_ReplacesTheImageUrl()
    {
        var attraction = NearbyAttraction.Create("Old City", "Landmark", 32m, 35m, "http://img/old.png");

        attraction.Update("Old City", "Landmark", 32m, 35m, " http://img/new.png ");

        Assert.Equal("http://img/new.png", attraction.AttractionImageUrl);
    }
}