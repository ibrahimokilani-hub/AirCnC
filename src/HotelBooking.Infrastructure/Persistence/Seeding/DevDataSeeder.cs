using HotelBooking.Core.Application.Abstractions;
using HotelBooking.Core.Domain.Entities;
using HotelBooking.Core.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace HotelBooking.Infrastructure.Persistence.Seeding;

/// <summary>
/// Dummy data for trying the API by hand in Development. Safe to run twice:
/// it only inserts rows that are not there yet. Chapter 6 adds hotels and rooms.
/// </summary>
public sealed class DevDataSeeder(
    AppDbContext context,
    IPasswordHasher passwordHasher,
    TimeProvider timeProvider,
    ILogger<DevDataSeeder> logger)
{
    public const string AdminEmail = "admin@kilani.com";
    public const string GuestEmail = "guest@kilani.com";

    private static readonly (string Name, string Country, string PostOffice)[] Cities =
    [
        ("Jerusalem", "Palestine", "P900"),
        ("Ramallah", "Palestine", "P600"),
        ("Nablus", "Palestine", "P400"),
        ("Hebron", "Palestine", "P700")
    ];

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        await SeedUsersAsync(cancellationToken);
        await SeedCitiesAsync(cancellationToken);
    }

    private async Task SeedUsersAsync(CancellationToken cancellationToken)
    {

        await EnsureUserAsync(AdminEmail, "Ada", "Admin", UserRole.Admin, "test123", cancellationToken);
        await EnsureUserAsync(GuestEmail, "Gus", "Guest", UserRole.User, "test123", cancellationToken);
    }

    private async Task EnsureUserAsync(
        string email,
        string firstName,
        string lastName,
        UserRole role,
        string password,
        CancellationToken cancellationToken)
    {
        if (await context.Users.AnyAsync(user => user.Email == email, cancellationToken))
        {
            return;
        }

        var user = User.Create(email, firstName, lastName, passwordHasher.Hash(password), timeProvider.GetUtcNow().UtcDateTime);
        user.Role = role;

        context.Users.Add(user);
        await context.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Seeded user {Email} with role {Role}", email, role);
    }

    private async Task SeedCitiesAsync(CancellationToken cancellationToken)
    {
        var existing = await context.Cities
            .Select(city => city.Name + "|" + city.Country)
            .ToListAsync(cancellationToken);

        var missing = Cities
            .Where(city => !existing.Contains(city.Name + "|" + city.Country))
            .Select(city => City.Create(city.Name, city.Country, city.PostOffice))
            .ToList();

        context.Cities.AddRange(missing);
        await context.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Seeded {Count} cities", missing.Count);
    }
}