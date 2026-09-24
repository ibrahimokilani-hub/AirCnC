using HotelBooking.Core.Domain.Common;

namespace HotelBooking.Core.Application.Features.Auth;

public static class AuthErrors
{
    public static Error EmailTaken(string email) =>
        Error.Conflict("Auth.EmailTaken", $"Email '{email}' already exists.");
    
    public static Error InvalidCreds =
        Error.Unauthorized("Auth.InvalidCreds", "Invalid email or password");
    
    public static Error NotAuthenticated =
        Error.Unauthorized("Auth.NotAuthenticated", "Please log in... ");
    
    public static readonly Error InvalidRefresh =
        Error.Unauthorized("Auth.InvalidRefreshToken", "Your session has expired. Please log in again.");

}