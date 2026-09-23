using System.Security.Claims;
using HotelBooking.Core.Application.Abstractions;
using HotelBooking.Core.Domain.Enums;
using HotelBooking.Infrastructure.Security;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
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

    public UserRole? Role
    {
        get
        {
            var value = httpContextAccessor.HttpContext?.User.FindFirst(AuthClaims.Role)?.Value;
            return value == "Admin" ? UserRole.Admin : UserRole.User;   // never returns null
        }
    }

}