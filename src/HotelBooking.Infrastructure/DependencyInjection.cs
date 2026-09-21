using HotelBooking.Core.Application.Abstractions;
using HotelBooking.Infrastructure.Persistence;
using HotelBooking.Infrastructure.Persistence.Interceptors;
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

        // each request's context gets that request's interceptor and ICurrentUser
        services.AddDbContext<AppDbContext>((provider, options) => options
            .UseSqlServer(connectionString)
            .AddInterceptors(provider.GetRequiredService<AuditingInterceptor>())
        );

        services.AddScoped<IAppDbContext>(provider => provider.GetRequiredService<AppDbContext>());

        return services;
    }
}