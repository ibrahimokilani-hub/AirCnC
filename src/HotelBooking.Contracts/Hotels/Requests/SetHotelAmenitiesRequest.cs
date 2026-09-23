namespace HotelBooking.Contracts.Hotels.Requests;

public sealed record SetHotelAmenitiesRequest(IReadOnlyList<int> AmenityIds);