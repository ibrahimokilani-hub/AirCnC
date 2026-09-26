namespace HotelBooking.Contracts.Cities.Responses;

public sealed record CityListItem(
    int Id,
    string Name,
    string Country,
    string PostOffice,
    string? CityImageUrl,
    int HotelsCount,
    DateTime CreatedAtUtc,
    DateTime? UpdatedAtUtc);