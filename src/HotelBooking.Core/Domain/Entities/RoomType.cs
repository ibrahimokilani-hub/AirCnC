using HotelBooking.Core.Domain.Common;

namespace HotelBooking.Core.Domain.Entities;

public sealed class RoomType : AuditableEntity
{
    public const int NameMaxLength = 100;
    public const int DescriptionMaxLength = 1000;
    public const int MaxGuestsPerRoom = 10;

    private RoomType(int hotelId, string name, string description, decimal pricePerNight, int maxAdults, int maxChildren)
    {
        HotelId = hotelId;
        Name = name;
        Description = description;
        PricePerNight = pricePerNight;
        MaxAdults = maxAdults;
        MaxChildren = maxChildren;
    }

    public int HotelId { get; private set; }

    public string Name { get; private set; }

    public string Description { get; private set; }

    public decimal PricePerNight { get; private set; }

    public int MaxAdults { get; private set; }

    public int MaxChildren { get; private set; }

    public static RoomType Create(
        int hotelId,
        string name,
        string description,
        decimal pricePerNight,
        int maxAdults,
        int maxChildren)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(hotelId);
        Guard(name, pricePerNight, maxAdults, maxChildren);

        return new RoomType(hotelId, name.Trim(), description.Trim(), pricePerNight, maxAdults, maxChildren);
    }

    public void Update(string name, string description, decimal pricePerNight, int maxAdults, int maxChildren)
    {
        Guard(name, pricePerNight, maxAdults, maxChildren);

        Name = name.Trim();
        Description = description.Trim();
        PricePerNight = pricePerNight;
        MaxAdults = maxAdults;
        MaxChildren = maxChildren;
    }

    public bool Fits(int adults, int children) => adults <= MaxAdults && children <= MaxChildren;

    private static void Guard(string name, decimal pricePerNight, int maxAdults, int maxChildren)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(pricePerNight);
        ArgumentOutOfRangeException.ThrowIfLessThan(maxAdults, 1);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(maxAdults, MaxGuestsPerRoom);
        ArgumentOutOfRangeException.ThrowIfNegative(maxChildren);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(maxChildren, MaxGuestsPerRoom);
    }
}
