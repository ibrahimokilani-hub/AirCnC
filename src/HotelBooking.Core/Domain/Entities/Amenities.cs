using HotelBooking.Core.Domain.Common;

namespace HotelBooking.Core.Domain.Entities;

public sealed class Amenity : AuditableEntity
{
    public const int NameMaxLength = 100;

    private Amenity(string name)
    {
        Name = name;
    }

    public string Name { get; private set; }

    public static Amenity Create(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        return new Amenity(name.Trim());
    }

    public void Rename(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        Name = name.Trim();
    }
}