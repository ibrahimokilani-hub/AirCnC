using Serilog;
using Serilog.Formatting.Compact;

namespace HotelBooking.Api.Extensions;

public static class LoggingExtensions
{
    public static WebApplicationBuilder AddSerilogLogging(this WebApplicationBuilder builder)
    {
        builder.Host.UseSerilog((context, services, logger) => logger
            .ReadFrom.Configuration(context.Configuration)
            .ReadFrom.Services(services)
            .Enrich.FromLogContext()
            .WriteTo.File("logs/hotelLogs.txt", rollingInterval: RollingInterval.Day));

        return builder;
    }
}