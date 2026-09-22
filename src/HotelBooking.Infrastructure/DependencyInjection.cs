using HotelBooking.Core.Application.Abstractions;
using HotelBooking.Core.Application.Features.Auth.Register;
using HotelBooking.Infrastructure.Identity;
using HotelBooking.Infrastructure.Persistence;
using HotelBooking.Infrastructure.Persistence.Interceptors;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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

        // Identity
        services.AddIdentityServices();

        return services;
    }
    
    private static IServiceCollection AddIdentityServices(this IServiceCollection services)
    {
       services
            .AddIdentityCore<ApplicationUser>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.Password.RequiredLength = 8;
            })
            .AddRoles<IdentityRole<int>>()
            .AddEntityFrameworkStores<AppDbContext>();

        services.AddScoped<IIdentityService, IdentityService>();

        return services;
    }
}