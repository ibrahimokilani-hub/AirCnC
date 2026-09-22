using System.Security.Claims;
using HotelBooking.Core.Application.Abstractions;
using Microsoft.IdentityModel.JsonWebTokens;

namespace HotelBooking.Api.Services;

public sealed class CurrentUser(IHttpContextAccessor httpContextAccessor) : ICurrentUser
{
    public int? UserId
    {
        get
        {
            var value = httpContextAccessor.HttpContext?.User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

            return int.TryParse(value, out var id) ? id : null;
        }
    }
}