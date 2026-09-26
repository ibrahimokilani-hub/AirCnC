namespace HotelBooking.Contracts.Hotels.Responses;

public sealed record NearbyAttractionResponse(
    int Id,
    string Name,
    string Category,
    decimal Latitude,
    decimal Longitude,
    string? AttractionImageUrl);
