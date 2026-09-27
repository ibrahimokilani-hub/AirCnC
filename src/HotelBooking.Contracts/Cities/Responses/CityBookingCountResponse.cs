namespace HotelBooking.Contracts.Cities.Responses;

public sealed record CityBookingCountResponse(
    CityResponse City,
    int BookingCount);