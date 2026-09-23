namespace HotelBooking.Contracts.Hotels.Responses;

public sealed record HotelListItem(
    int Id,
    string Name,
    string CityName,
    int StarRating,
    string OwnerName,
    string HotelType,
    DateTime CreatedAtUtc,
    DateTime? UpdatedAtUtc);