namespace HotelBooking.Api.Services;

public static class RefreshTokenCookie
{
    public const string Name = "refresh_token";

    public const string Path = "/api/v1/auth";

    public static void Write(HttpResponse response, string token, DateTime expiresAtUtc) =>
        response.Cookies.Append(Name, token, Options(expiresAtUtc));

    public static string? Read(HttpRequest request) =>
        request.Cookies.TryGetValue(Name, out var token) ? token : null;

    public static void Delete(HttpResponse response) =>
        response.Cookies.Delete(Name, Options(expiresAtUtc: null));

    private static CookieOptions Options(DateTime? expiresAtUtc) => new()
    {
        HttpOnly = true,                 
        Secure = true,                  
        SameSite = SameSiteMode.Strict,  
        Path = Path,
        Expires = expiresAtUtc
    };
}