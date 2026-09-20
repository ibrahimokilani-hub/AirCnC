namespace HotelBooking.Contracts.Cities.Requests;

public sealed record CreateCityRequest(string Name, string Country, string PostOffice);
