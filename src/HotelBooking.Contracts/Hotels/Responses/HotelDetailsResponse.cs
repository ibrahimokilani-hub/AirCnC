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
    string? HotelImageUrl,
    IReadOnlyList<string> Amenities,
    IReadOnlyList<NearbyAttractionResponse> NearbyAttractions,
    double? AverageRating,
    int ReviewsCount);

public sealed record NearbyAttractionResponse(
    int Id,
    string Name,
    string Category,
    decimal Latitude,
    decimal Longitude,
    int DistanceMeters,
    string? AttractionImageUrl);