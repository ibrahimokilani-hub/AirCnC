using HotelBooking.Core.Domain.Common;

namespace HotelBooking.Core.Domain.Entities;

public sealed class NearbyAttraction : Entity
{
    public const int NameMaxLength = 150;
    public const int CategoryMaxLength = 50;

    private NearbyAttraction(string name, string category, decimal latitude, decimal longitude, string? attractionImageUrl)
    {
        Name = name;
        Category = category;
        Latitude = latitude;
        Longitude = longitude;
        AttractionImageUrl = attractionImageUrl;
    }

    public string Name { get; private set; }

    public string Category { get; private set; }

    public decimal Latitude { get; private set; }

    public decimal Longitude { get; private set; }

    public string? AttractionImageUrl { get; private set; }

    public static NearbyAttraction Create(string name, string category, decimal latitude, decimal longitude, string? attractionImageUrl = null)
    {
        Guard(name, category, latitude, longitude);

        return new NearbyAttraction(name.Trim(), category.Trim(), latitude, longitude, attractionImageUrl?.Trim());
    }

    public void Update(string name, string category, decimal latitude, decimal longitude, string? attractionImageUrl)
    {
        Guard(name, category, latitude, longitude);

        Name = name.Trim();
        Category = category.Trim();
        Latitude = latitude;
        Longitude = longitude;
        AttractionImageUrl = attractionImageUrl?.Trim();
    }

    private static void Guard(string name, string category, decimal latitude, decimal longitude)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(category);
        ArgumentOutOfRangeException.ThrowIfLessThan(latitude, -90m);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(latitude, 90m);
        ArgumentOutOfRangeException.ThrowIfLessThan(longitude, -180m);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(longitude, 180m);
    }
}
