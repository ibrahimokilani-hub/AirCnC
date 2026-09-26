namespace HotelBooking.Contracts.Hotels.Responses;

public sealed record HotelListItem(
    int Id,
    string Name,
    string CityName,
    int StarRating,
    string OwnerName,
    string HotelType,
    string? HotelImageUrl,
    int RoomsCount,
    DateTime CreatedAtUtc,
    DateTime? UpdatedAtUtc);