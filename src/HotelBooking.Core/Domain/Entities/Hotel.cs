using HotelBooking.Core.Domain.Common;
using HotelBooking.Core.Domain.Enums;

namespace HotelBooking.Core.Domain.Entities;

public sealed class Hotel : AuditableEntity
{
    public const int NameMaxLength = 200;
    public const int DescriptionMaxLength = 4000;
    public const int AddressMaxLength = 300;
    public const int MinStars = 1;
    public const int MaxStars = 5;
    
    private readonly List<RoomType> _roomTypes = [];
    private readonly List<Room> _rooms = [];
    private readonly List<Amenity> _amenities = [];

    private Hotel(
        int cityId,
        int ownerId,
        string name,
        string description,
        int starRating,
        HotelType hotelType,
        string address,
        decimal latitude,
        decimal longitude)
    {
        CityId = cityId;
        OwnerId = ownerId;
        Name = name;
        Description = description;
        StarRating = starRating;
        HotelType = hotelType;
        Address = address;
        Latitude = latitude;
        Longitude = longitude;
    }

    public int CityId { get; private set; }

    public City City { get; private set; } = null!;
    
    public int OwnerId { get; private set; }

    /// <summary>Loaded only when a query asks for it, like City.</summary>
    public User Owner { get; private set; } = null!;

    public string Name { get; private set; }

    public string Description { get; private set; }

    public int StarRating { get; private set; }

    public HotelType HotelType { get; private set; }

    public string Address { get; private set; }

    public decimal Latitude { get; private set; }

    public decimal Longitude { get; private set; }
    
    public IReadOnlyCollection<RoomType> RoomTypes => _roomTypes;
    public IReadOnlyCollection<Room> Rooms => _rooms;
    public IReadOnlyCollection<Amenity> Amenities => _amenities;
    
    public void SetAmenities(IEnumerable<Amenity> amenities)
    {
        _amenities.Clear();
        _amenities.AddRange(amenities.Distinct());
    }


    public static Hotel Create(
        int cityId,
        int ownerId,
        string name,
        string description,
        int starRating,
        HotelType hotelType,
        string address,
        decimal latitude,
        decimal longitude)
    {
        Guard(cityId, ownerId, name, address, starRating, latitude, longitude);

        return new Hotel(
            cityId,
            ownerId,
            name.Trim(),
            description.Trim(),
            starRating,
            hotelType,
            address.Trim(),
            latitude,
            longitude);
    }

    /// <summary>OwnerId is deliberately not a parameter: ownership does not change here.</summary>
    public void Update(
        int cityId,
        string name,
        string description,
        int starRating,
        HotelType hotelType,
        string address,
        decimal latitude,
        decimal longitude)
    {
        Guard(cityId, OwnerId, name, address, starRating, latitude, longitude);

        CityId = cityId;
        Name = name.Trim();
        Description = description.Trim();
        StarRating = starRating;
        HotelType = hotelType;
        Address = address.Trim();
        Latitude = latitude;
        Longitude = longitude;
    }

    private static void Guard(
        int cityId,
        int ownerId,
        string name,
        string address,
        int starRating,
        decimal latitude,
        decimal longitude)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(cityId);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(ownerId);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(address);
        ArgumentOutOfRangeException.ThrowIfLessThan(starRating, MinStars);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(starRating, MaxStars);
        ArgumentOutOfRangeException.ThrowIfLessThan(latitude, -90m);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(latitude, 90m);
        ArgumentOutOfRangeException.ThrowIfLessThan(longitude, -180m);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(longitude, 180m);
    }
}
