namespace HotelBooking.Contracts.Hotels.Responses;

public sealed record HotelListItem(
    int Id,
    string Name,
    string CityName,
    int StarRating,
    string OwnerName,
    string HotelType,
    int RoomsCount,
    DateTime CreatedAtUtc,
    DateTime? UpdatedAtUtc);