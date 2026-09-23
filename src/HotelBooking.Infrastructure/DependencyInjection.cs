using HotelBooking.Core.Application.Abstractions;
using HotelBooking.Infrastructure.Options;
using HotelBooking.Infrastructure.Persistence;
using HotelBooking.Infrastructure.Persistence.Interceptors;
using HotelBooking.Infrastructure.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace HotelBooking.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("HotelBookingDb")
                               ?? throw new InvalidOperationException(
                                   "Connection string 'HotelBookingDb' is missing.");
        
        services.AddSingleton(TimeProvider.System);
        services.AddScoped<AuditingInterceptor>();

        // each request's context gets that request's interceptor and ICurrentUser
        services.AddDbContext<AppDbContext>((provider, options) => options
            .UseSqlServer(connectionString)
            .AddInterceptors(provider.GetRequiredService<AuditingInterceptor>()));

        services.AddScoped<IAppDbContext>(provider => provider.GetRequiredService<AppDbContext>());

        // Stateless and thread-safe: one instance for the whole app.
        services.AddSingleton<IPasswordHasher, Pbkdf2PasswordHasher>();

        services.AddJwtAuthentication();

        return services;
    }
 private static IServiceCollection AddJwtAuthentication(this IServiceCollection services)
    {
        services.AddOptions<JwtOptions>()
            .BindConfiguration(JwtOptions.SectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddSingleton<ITokenService, JwtTokenService>();
        services.AddScoped<IRefreshTokenService, RefreshTokenService>();

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();

        services.AddOptions<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme)
            .Configure<IOptions<JwtOptions>, TimeProvider>((bearer, jwtOptions, timeProvider) =>
            {
                var jwt = jwtOptions.Value;

                bearer.MapInboundClaims = false;

                bearer.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwt.Issuer,
                    ValidateAudience = true,
                    ValidAudience = jwt.Audience,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = JwtTokenService.CreateSigningKey(jwt.SecretKey),
                    ValidAlgorithms = [SecurityAlgorithms.HmacSha256],
                    NameClaimType = JwtRegisteredClaimNames.Sub,
                    RoleClaimType = AuthClaims.Role,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromSeconds(30),
                    
                    LifetimeValidator = (notBefore, expires, _, parameters) =>
                    {
                        var now = timeProvider.GetUtcNow().UtcDateTime;
                        return (notBefore is null || notBefore.Value <= now + parameters.ClockSkew)
                               && expires is not null
                               && expires.Value > now - parameters.ClockSkew;
                    }
                };
            });

        return services;
    }
}