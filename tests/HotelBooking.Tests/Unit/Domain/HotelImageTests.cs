using HotelBooking.Core.Domain.Entities;

namespace HotelBooking.Tests.Unit.Domain;

public sealed class HotelImageTests
{
    [Fact]
    public void Create_TrimsTheUrl() =>
        Assert.Equal("http://img/1.png", HotelImage.Create(1, " http://img/1.png ").ImageUrl);

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Create_BlankUrl_Throws(string imageUrl) =>
        Assert.Throws<ArgumentException>(() => HotelImage.Create(1, imageUrl));

    /// <summary>A gallery image belongs to a saved hotel, so it always has a real hotel id.</summary>
    [Fact]
    public void Create_WithoutAHotel_Throws() =>
        Assert.Throws<ArgumentOutOfRangeException>(() => HotelImage.Create(0, "http://img/1.png"));

    [Fact]
    public void Update_ReplacesAndTrimsTheUrl()
    {
        var image = HotelImage.Create(1, "http://img/1.png");

        image.Update(" http://img/2.png ");

        Assert.Equal("http://img/2.png", image.ImageUrl);
    }
}