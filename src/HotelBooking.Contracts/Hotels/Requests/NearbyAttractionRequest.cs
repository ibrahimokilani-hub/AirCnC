namespace HotelBooking.Contracts.Hotels.Requests;

public sealed record NearbyAttractionRequest(string Name, string Category, decimal Latitude, decimal Longitude);