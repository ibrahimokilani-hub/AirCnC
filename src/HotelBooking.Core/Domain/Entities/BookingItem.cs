using HotelBooking.Core.Domain.Common;
using HotelBooking.Core.Domain.ValueObjects;

namespace HotelBooking.Core.Domain.Entities;

public sealed class BookingItem : Entity
{
    private BookingItem(int roomId, DateOnly checkIn, DateOnly checkOut, int adults, int children, decimal pricePerNight)
    {
        RoomId = roomId;
        CheckIn = checkIn;
        CheckOut = checkOut;
        Adults = adults;
        Children = children;
        PricePerNight = pricePerNight;
        Nights = checkOut.DayNumber - checkIn.DayNumber;
        IsActive = true;
    }

    public int BookingId { get; private set; }

    public int RoomId { get; private set; }

    public DateOnly CheckIn { get; private set; }

    public DateOnly CheckOut { get; private set; }

    public int Adults { get; private set; }

    public int Children { get; private set; }

    public decimal PricePerNight { get; private set; }

    public int Nights { get; private set; }

    public bool IsActive { get; private set; }

    public DateRange Stay => DateRange.Create(CheckIn, CheckOut);

    internal static BookingItem Create(int roomId, DateRange stay, int adults, int children, decimal pricePerNight)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(roomId);
        ArgumentOutOfRangeException.ThrowIfLessThan(adults, 1);
        ArgumentOutOfRangeException.ThrowIfNegative(children);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(pricePerNight);

        return new BookingItem(roomId, stay.CheckIn, stay.CheckOut, adults, children, pricePerNight);
    }

    public void Deactivate() => IsActive = false;
}