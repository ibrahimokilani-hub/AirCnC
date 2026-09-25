using HotelBooking.Core.Domain.Common;
using HotelBooking.Core.Domain.Enums;
using HotelBooking.Core.Domain.ValueObjects;

namespace HotelBooking.Core.Domain.Entities;

public sealed class Booking : Entity
{
    public const string ConfirmationPrefix = "HB-";

    private readonly List<BookingItem> _items = [];

    private Booking(
        int userId,
        int hotelId,
        string? notes,
        DateTime createdAtUtc)
    {
        UserId = userId;
        HotelId = hotelId;
        Notes = notes;
        CreatedAtUtc = createdAtUtc;
        Status = BookingStatus.Confirmed;
    }

    public int UserId { get; private set; }

    public int HotelId { get; private set; }

    public string ConfirmationNumber => ConfirmationPrefix + Id;

    public BookingStatus Status { get; private set; }

    public string? Notes { get; private set; }

    public decimal TotalPrice { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }

    public IReadOnlyCollection<BookingItem> Items => _items;

    public static Booking Create(
        int userId,
        int hotelId,
        string? notes,
        DateTime createdAtUtc)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(userId);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(hotelId);

        return new Booking(
            userId,
            hotelId,
            string.IsNullOrWhiteSpace(notes) ? null : notes.Trim(),
            createdAtUtc);
    }

    public BookingItem AddItem(int roomId, DateRange stay, int adults, int children, decimal pricePerNight)
    {
        var item = BookingItem.Create(roomId, stay, adults, children, pricePerNight);

        _items.Add(item);
        TotalPrice += item.PricePerNight * item.Nights;

        return item;
    }
}
