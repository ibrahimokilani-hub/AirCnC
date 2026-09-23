namespace HotelBooking.Contracts.Hotels.Requests;

public sealed record HotelRequest(
    int CityId,
    string Name,
    string Description,
    int StarRating,
    string HotelType,
    string Address,
    decimal Latitude,
    decimal Longitude);