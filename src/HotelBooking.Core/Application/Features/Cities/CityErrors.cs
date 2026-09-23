using HotelBooking.Core.Domain.Common;

namespace HotelBooking.Core.Application.Features.Cities;

public static class CityErrors
{
    public static Error AlreadyExists(string name, string country) =>
        Error.Conflict("City.AlreadyExists", $"The city '{name}, {country}' already exists.");

    public static Error NotFound(int id) =>
        Error.NotFound("City.NotFound", $"The city with ID '{id}' was not found.");
    
    public static Error HasHotels(int id) =>
        Error.Conflict("City.HasHotels", $"The city with ID '{id}' still has hotels. Delete or move them first.");

}