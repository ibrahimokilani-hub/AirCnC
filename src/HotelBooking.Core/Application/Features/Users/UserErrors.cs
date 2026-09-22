using HotelBooking.Core.Domain.Common;

namespace HotelBooking.Core.Application.Features.Users;

public static class UserErrors
{
    public static Error NotFound(int id) =>
        Error.NotFound("User.NotFound", $"The user with ID '{id}' was not found.");
}