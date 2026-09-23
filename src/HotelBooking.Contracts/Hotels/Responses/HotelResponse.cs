namespace HotelBooking.Contracts.Hotels.Responses;

public sealed record HotelResponse(
    int Id,
    int CityId,
    string CityName,
    string Name,
    int OwnerId,
    string OwnerName,
    string Description,
    int StarRating,
    string HotelType,
    string Address,
    decimal Latitude,
    decimal Longitude);