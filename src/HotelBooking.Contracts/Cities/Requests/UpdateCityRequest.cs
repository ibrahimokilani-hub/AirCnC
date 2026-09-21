namespace HotelBooking.Contracts.Cities.Requests;

public sealed record UpdateCityRequest(string Name, string Country, string PostOffice);