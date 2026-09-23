namespace HotelBooking.Contracts.Cities.Responses;

public sealed record CityListItem(
    int Id,
    string Name,
    string Country,
    string PostOffice,
    int HotelsCount,
    DateTime CreatedAtUtc,
    DateTime? UpdatedAtUtc);