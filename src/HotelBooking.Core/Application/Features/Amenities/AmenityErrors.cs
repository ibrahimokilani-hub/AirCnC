using HotelBooking.Core.Domain.Common;

namespace HotelBooking.Core.Application.Features.Amenities;

public static class AmenityErrors
{
    public static Error NotFound(int id) =>
        Error.NotFound("Amenity.NotFound", $"The amenity with ID '{id}' was not found.");

    public static Error AlreadyExists(string name) =>
        Error.Conflict("Amenity.AlreadyExists", $"The amenity '{name}' already exists.");

    public static ValidationError Unknown(IEnumerable<int> ids) =>
        new(new Dictionary<string, string[]>
        {
            ["AmenityIds"] = [$"These amenities do not exist: {string.Join(", ", ids)}."]
        });
}