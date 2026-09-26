using HotelBooking.Core.Domain.Common;

namespace HotelBooking.Core.Domain.Entities;

public sealed class Amenity : AuditableEntity
{
    public const int NameMaxLength = 100;

    private Amenity(string name, string? amenityImageUrl = null)
    {
        Name = name;
        AmenityImageUrl = amenityImageUrl;
    }

    public string Name { get; private set; }
    public string? AmenityImageUrl { get; private set; }

    public static Amenity Create(string name, string? amenityImageUrl = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        return new Amenity(name.Trim(), amenityImageUrl?.Trim());
    }

    public void Rename(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        Name = name.Trim();
    }
}