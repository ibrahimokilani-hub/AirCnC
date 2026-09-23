using HotelBooking.Core.Domain.Common;

namespace HotelBooking.Core.Application.Features.Hotels;

public static class HotelErrors
{
    public static Error NotFound(int id) =>
        Error.NotFound("Hotel.NotFound", $"The hotel with ID '{id}' was not found.");

   public static Error NotYours(int id) =>
        Error.Forbidden("Hotel.NotYours", $"The hotel with ID '{id}' belongs to another owner.");
   
   public static Error HasRooms(int id) =>
       Error.Conflict("Hotel.HasRooms", $"The hotel with ID '{id}' still has rooms. Delete them first.");

 public static ValidationError UnknownCity(int cityId) =>
        new(new Dictionary<string, string[]>
        {
            ["CityId"] = [$"The city with ID '{cityId}' does not exist."]
        });
}