using System.Security.Claims;
using Microsoft.IdentityModel.JsonWebTokens; // Use the modern package namespace here too!
using HotelBooking.Core.Application.Abstractions;

namespace HotelBooking.Infrastructure.Security;

public static class ClaimsCreator
{
    public static List<Claim> GenerateClaims(AuthUser user)
    {
        return new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Name, $"{user.FirstName} {user.LastName}"),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };
    }
}