using HotelBooking.Api.Extensions;
using HotelBooking.Core;
using HotelBooking.Infrastructure;
using HotelBooking.Infrastructure.Persistence;
using HotelBooking.Infrastructure.Persistence.Seeding;
using Microsoft.EntityFrameworkCore;
using Serilog;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.AddSerilogLogging();

builder.Services.AddPresentation()
                .AddInfrastructure(builder.Configuration)
                .AddCore();

builder.Services.AddScoped<DevDataSeeder>();

var app = builder.Build();

// Dev-only data bootstrap:
//   --reseed : apply migrations, then add any missing dummy data (additive, idempotent)
//   --reset  : apply migrations, wipe every row, flush the Redis cache, then reseed clean
if (app.Environment.IsDevelopment() &&
    (args.Contains("--reseed", StringComparer.OrdinalIgnoreCase) ||
     args.Contains("--reset", StringComparer.OrdinalIgnoreCase)))
{
    var reset = args.Contains("--reset", StringComparer.OrdinalIgnoreCase);

    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await context.Database.MigrateAsync();

    var seeder = scope.ServiceProvider.GetRequiredService<DevDataSeeder>();

    if (reset)
    {
        await seeder.ClearAsync();
        await FlushRedisAsync(builder.Configuration, app.Logger);
    }

    await seeder.SeedAsync();
}

app.UseApiPipeline();

app.Run();

// Clears the app's Redis cache so a fresh dataset can't collide with entries cached against the
// old one. No-op when no Redis connection string is set (the in-memory cache fallback). FLUSHDB
// is an admin command, so the connection opts into admin mode just for this one-off call.
static async Task FlushRedisAsync(IConfiguration configuration, Microsoft.Extensions.Logging.ILogger logger)
{
    var connectionString = configuration.GetConnectionString("Redis");

    if (string.IsNullOrWhiteSpace(connectionString))
    {
        return;
    }

    var options = ConfigurationOptions.Parse(connectionString);
    options.AllowAdmin = true;

    using var connection = await ConnectionMultiplexer.ConnectAsync(options);

    foreach (var endpoint in connection.GetEndPoints())
    {
        await connection.GetServer(endpoint).FlushDatabaseAsync();
    }

    logger.LogInformation("Flushed the Redis cache.");
}
