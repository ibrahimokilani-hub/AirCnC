namespace HotelBooking.Core.Domain.ValueObjects;


public readonly record struct DateRange
{
    public const int MaxNights = 30;

    private DateRange(DateOnly checkIn, DateOnly checkOut)
    {
        CheckIn = checkIn;
        CheckOut = checkOut;
    }

    public DateOnly CheckIn { get; }

    public DateOnly CheckOut { get; }

    public int Nights => CheckOut.DayNumber - CheckIn.DayNumber;

    public static DateRange Create(DateOnly checkIn, DateOnly checkOut)
    {
        if (checkOut <= checkIn)
        {
            throw new ArgumentException("Check-out must be after check-in.", nameof(checkOut));
        }

        return new DateRange(checkIn, checkOut);
    }

    public bool Overlaps(DateRange other) => CheckIn < other.CheckOut && other.CheckIn < CheckOut;

    public bool Contains(DateOnly day) => CheckIn <= day && day < CheckOut;
}