using System.Security.Claims;
using System.Text;
using HotelBooking.Core.Application.Abstractions;
using HotelBooking.Core.Domain.Entities;
using HotelBooking.Infrastructure.Options;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace HotelBooking.Infrastructure.Security;

public class JwtTokenService(IOptions<JwtOptions> options, TimeProvider timeProvider) : ITokenService
{
    private readonly JwtOptions _options = options.Value;
    private readonly JsonWebTokenHandler _jsonWebTokenHandler = new();

    public AccessToken CreateAccessToken(AuthUser user)
    {
        var now = timeProvider.GetUtcNow().UtcDateTime;
        var expires = now.AddMinutes(_options.AccessTokenMinutes);

        var claims = ClaimsCreator.GenerateClaims(user);

        var descriptor = new SecurityTokenDescriptor
        {
            Issuer = _options.Issuer,
            Audience = _options.Audience,
            Subject = new ClaimsIdentity(claims), 
            Expires = expires,
            NotBefore = now,
            IssuedAt = now,
            SigningCredentials = new SigningCredentials(CreateSigningKey(_options.SecretKey), SecurityAlgorithms.HmacSha256) 
        };
        
        string tokenString = _jsonWebTokenHandler.CreateToken(descriptor);
        return new AccessToken(tokenString, expires);
    }

    public static SymmetricSecurityKey CreateSigningKey(string secretKey) => 
        new(Encoding.UTF8.GetBytes(secretKey));
}

public static class AuthClaims
{
    public const string Role = "role";
}