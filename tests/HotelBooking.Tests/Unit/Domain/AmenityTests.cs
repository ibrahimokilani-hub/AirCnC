using HotelBooking.Core.Domain.Entities;

namespace HotelBooking.Tests.Unit.Domain;

public sealed class AmenityTests
{
    [Theory]
    [InlineData(" http://img/pool.png ", "http://img/pool.png")]
    [InlineData(null, null)]
    public void Create_ImageUrl_IsTrimmedOrNull(string? imageUrl, string? expected) =>
        Assert.Equal(expected, Amenity.Create("Pool", imageUrl).AmenityImageUrl);

    /// <summary>Rename takes only a name, so the image survives it. There is no other way to change it.</summary>
    [Fact]
    public void Rename_KeepsTheImageUrl()
    {
        var amenity = Amenity.Create("Pool", "http://img/pool.png");

        amenity.Rename("Swimming pool");

        Assert.Equal("http://img/pool.png", amenity.AmenityImageUrl);
    }
}