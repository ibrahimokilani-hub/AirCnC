using HotelBooking.Core.Domain.Common;

namespace HotelBooking.Core.Domain.Entities;

/// <summary>
/// A physical room: a number on a door. Price and capacity come from its RoomType.
/// Availability is never stored here; it's computed from bookings (chapter 7).
/// </summary>
public sealed class Room : AuditableEntity
{
    public const int NumberMaxLength = 20;

    private Room(int hotelId, int roomTypeId, string number)
    {
        HotelId = hotelId;
        RoomTypeId = roomTypeId;
        Number = number;
    }

    public int HotelId { get; private set; }

    public int RoomTypeId { get; private set; }

    public RoomType RoomType { get; private set; } = null!;

    public string Number { get; private set; }

    public static Room Create(int hotelId, int roomTypeId, string number)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(hotelId);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(roomTypeId);
        ArgumentException.ThrowIfNullOrWhiteSpace(number);

        return new Room(hotelId, roomTypeId, number.Trim());
    }

    public void Update(int roomTypeId, string number)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(roomTypeId);
        ArgumentException.ThrowIfNullOrWhiteSpace(number);

        RoomTypeId = roomTypeId;
        Number = number.Trim();
    }
}