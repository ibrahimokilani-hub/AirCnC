using HotelBooking.Core.Domain.Common;

namespace HotelBooking.Core.Application.Features.Auth;

public static class AuthErrors
{
    public static Error EmailTaken(string email) =>
        Error.Conflict("Auth.EmailTaken", $"An account with the email '{email}' already exists.");
    
    public static Error InvalidCreds =
        Error.Unauthorized("Auth.InvalidCreds", "Invalid email or password");
    
    public static Error NotAuthenticated =
        Error.Unauthorized("Auth.NotAuthenticated", "Please log in... ");
    
    
}