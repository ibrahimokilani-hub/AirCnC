using HotelBooking.Core.Domain.Common;

namespace HotelBooking.Core.Domain.Entities;

public sealed class NearbyAttraction : Entity
{
    public const int NameMaxLength = 150;
    public const int CategoryMaxLength = 50;

    private NearbyAttraction(string name, string category, decimal latitude, decimal longitude, int distanceMeters)
    {
        Name = name;
        Category = category;
        Latitude = latitude;
        Longitude = longitude;
        DistanceMeters = distanceMeters;
    }

    public int HotelId { get; private set; }

    public string Name { get; private set; }

    public string Category { get; private set; }

    public decimal Latitude { get; private set; }

    public decimal Longitude { get; private set; }

    public int DistanceMeters { get; private set; }

    internal static NearbyAttraction Create(
        string name,
        string category,
        decimal latitude,
        decimal longitude,
        int distanceMeters)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(category);

        return new NearbyAttraction(name.Trim(), category.Trim(), latitude, longitude, distanceMeters);
    }
}