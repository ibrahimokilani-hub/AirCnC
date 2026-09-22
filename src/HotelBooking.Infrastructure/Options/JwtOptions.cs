using System.ComponentModel.DataAnnotations;

namespace HotelBooking.Infrastructure.Options;

public class JwtOptions
{
    public const string SectionName = "JWT";

    [Required] public string Issuer { get; init; } = string.Empty;
    [Required] public string Audience { get; init; } = string.Empty;
    [Required] public string SecretKey { get; init; } = string.Empty;
    
    public int AccessTokenMinutes { get; init; }
}