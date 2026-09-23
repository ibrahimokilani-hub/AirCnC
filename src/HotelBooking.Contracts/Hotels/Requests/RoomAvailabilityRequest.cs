namespace HotelBooking.Contracts.Hotels.Requests;

public sealed class RoomAvailabilityRequest
{
    public DateOnly? CheckIn { get; init; }

    public DateOnly? CheckOut { get; init; }

    public int Adults { get; init; } = 2;

    public int Children { get; init; }
}