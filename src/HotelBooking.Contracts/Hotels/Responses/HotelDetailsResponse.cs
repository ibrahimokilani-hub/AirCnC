namespace HotelBooking.Contracts.Hotels.Responses;

public sealed record HotelDetailsResponse(
    int Id,
    string Name,
    string CityName,
    string Country,
    int StarRating,
    string HotelType,
    string Description,
    string Address,
    decimal Latitude,
    decimal Longitude,
    IReadOnlyList<string> Amenities,
    IReadOnlyList<NearbyAttractionResponse> NearbyAttractions);

public sealed record NearbyAttractionResponse(
    int Id,
    string Name,
    string Category,
    decimal Latitude,
    decimal Longitude,
    int DistanceMeters);