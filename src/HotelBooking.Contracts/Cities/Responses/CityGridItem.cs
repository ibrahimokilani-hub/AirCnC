namespace HotelBooking.Contracts.Cities.Responses;

public sealed record CityGridItem(
    int Id,
    string Name,
    string Country,
    string PostOffice,
    DateTime CreatedAtUtc,
    DateTime? UpdatedAtUtc);