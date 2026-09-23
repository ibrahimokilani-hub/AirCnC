using HotelBooking.Core.Domain.Common;
using HotelBooking.Core.Domain.Enums;
using HotelBooking.Core.Domain.ValueObjects;

namespace HotelBooking.Core.Domain.Entities;

public sealed class Booking : Entity
{
    public const int ConfirmationNumberLength = 16;

    private readonly List<BookingItem> _items = [];
    
    private Booking(
        int userId,
        int hotelId,
        string confirmationNumber,
        string guestFirstName,
        string guestLastName,
        string guestEmail,
        string guestPhone,
        string? specialRequests,
        DateTime createdAtUtc)
    {
        UserId = userId;
        HotelId = hotelId;
        ConfirmationNumber = confirmationNumber;
        GuestFirstName = guestFirstName;
        GuestLastName = guestLastName;
        GuestEmail = guestEmail;
        GuestPhone = guestPhone;
        SpecialRequests = specialRequests;
        CreatedAtUtc = createdAtUtc;
        Status = BookingStatus.Confirmed;
    }

    public int UserId { get; private set; }

    public int HotelId { get; private set; }

    public string ConfirmationNumber { get; private set; }

    public BookingStatus Status { get; private set; }

    public string GuestFirstName { get; private set; }

    public string GuestLastName { get; private set; }

    public string GuestEmail { get; private set; }

    public string GuestPhone { get; private set; }

    public string? SpecialRequests { get; private set; }

    public decimal TotalPrice { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }

    public IReadOnlyCollection<BookingItem> Items => _items;

    public static Booking Create(
        int userId,
        int hotelId,
        string confirmationNumber,
        GuestDetails guest,
        string? specialRequests,
        DateTime createdAtUtc)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(userId);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(hotelId);
        ArgumentException.ThrowIfNullOrWhiteSpace(confirmationNumber);
        ArgumentException.ThrowIfNullOrWhiteSpace(guest.FirstName);
        ArgumentException.ThrowIfNullOrWhiteSpace(guest.LastName);
        ArgumentException.ThrowIfNullOrWhiteSpace(guest.Email);

        return new Booking(
            userId,
            hotelId,
            confirmationNumber,
            guest.FirstName.Trim(),
            guest.LastName.Trim(),
            guest.Email.Trim(),
            guest.Phone.Trim(),
            string.IsNullOrWhiteSpace(specialRequests) ? null : specialRequests.Trim(),
            createdAtUtc);
    }

    /// <summary>Books one room for one stay, copying today's price into the booking.</summary>
    public BookingItem AddItem(int roomId, DateRange stay, int adults, int children, decimal pricePerNight)
    {
        var item = BookingItem.Create(roomId, stay, adults, children, pricePerNight);

        _items.Add(item);
        TotalPrice += item.PricePerNight * item.Nights;

        return item;
    }
}
