using HotelBooking.Core.Domain.Entities;

namespace HotelBooking.Tests.Unit.Domain;

public sealed class CityTests
{
    [Fact]
    public void Create_TrimsEveryField()
    {
        var city = City.Create(" Jenin ", " Palestine ", " 11111 ");
        Assert.Equal("Jenin", city.Name);
        Assert.Equal("Palestine", city.Country);
        Assert.Equal("11111", city.PostOffice);
    } 
    
    [Fact] 
    public void Create_HasNoIdUntilSaved() 
    { 
        var city = City.Create("Jenin", "Palestine", "P100");
        Assert.Equal(0, city.Id); 
    }

    [Theory]
    [InlineData("", "Palestine", "P100")]
    [InlineData("Jenin", " ", "P100")]
    [InlineData("Jenin", "Palestine", "")]
    public void Create_BlankField_Throws(string name, string country, string postOffice)
    {
        Assert.Throws<ArgumentException>( () => City.Create(name, country, postOffice));
    }

    [Fact]
    public void Update_ReplacesAndTrimsEveryField()
    {
        var city = City.Create("Jenin", "Palestine", "P100"); 
        city.Update(" Nablus ", "Palestine", " P200 "); 
        Assert.Equal("Nablus", city.Name);
        Assert.Equal("P200", city.PostOffice);
    }

    [Fact]
    public void Update_BlankName_ThrowsAndLeavesTheCityUnchanged()
    {
        var city = City.Create("Jenin", "Palestine", "P100"); 
        Assert.Throws<ArgumentException>( () => city.Update("", "Palestine", "P100"));
        Assert.Equal("Jenin", city.Name);
    } 

    [Theory]
    [InlineData(" http://img/jenin.png ", "http://img/jenin.png")]
    [InlineData(null, null)]
    public void Create_ImageUrl_IsTrimmedOrNull(string? imageUrl, string? expected) =>
        Assert.Equal(expected, City.Create("Jenin", "Palestine", "P100", imageUrl).CityImageUrl);

    /// <summary>The PUT replaces every field, so an update with no image means "remove it".</summary>
    [Fact]
    public void Update_WithoutAnImageUrl_ClearsIt()
    {
        var city = City.Create("Jenin", "Palestine", "P100", "http://img/jenin.png");

        city.Update("Jenin", "Palestine", "P100");

        Assert.Null(city.CityImageUrl);
    }
}