using HotelBooking.Core.Application.Abstractions;
using HotelBooking.Core.Domain.Entities;
using HotelBooking.Core.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HotelBooking.Infrastructure.Persistence.Seeding;

/// <summary>
/// Small, fixed development dataset for manually testing the API.
/// Safe to run multiple times; existing records are not inserted again.
/// </summary>
public sealed class DevDataSeeder(
    AppDbContext context,
    IPasswordHasher passwordHasher,
    TimeProvider timeProvider,
    ILogger<DevDataSeeder> logger)
{
    public const string AdminEmail = "admin@hotelbooking.local";
    public const string GuestEmail = "guest@hotelbooking.local";

    public async Task SeedAsync(
        CancellationToken cancellationToken = default)
    {
        await SeedUsersAsync(cancellationToken);
        await SeedCitiesAsync(cancellationToken);
        await SeedAmenitiesAsync(cancellationToken);
        await SeedHotelsAsync(cancellationToken);
    }

    private async Task SeedUsersAsync(
        CancellationToken cancellationToken)
    {
        await EnsureUserAsync(
            AdminEmail,
            "Ada",
            "Admin",
            UserRole.Admin,
            cancellationToken);

        await EnsureUserAsync(
            GuestEmail,
            "Gus",
            "Guest",
            UserRole.User,
            cancellationToken);
    }

    private async Task EnsureUserAsync(
        string email,
        string firstName,
        string lastName,
        UserRole role,
        CancellationToken cancellationToken)
    {
        if (await context.Users.AnyAsync(
                user => user.Email == email,
                cancellationToken))
        {
            return;
        }

        var user = User.Create(
            email,
            firstName,
            lastName,
            passwordHasher.Hash("test123"),
            timeProvider.GetUtcNow().UtcDateTime);

        user.Role = role;

        context.Users.Add(user);

        await context.SaveChangesAsync(cancellationToken);

        logger.LogInformation(
            "Seeded user {Email} with role {Role}",
            email,
            role);
    }

    private async Task SeedCitiesAsync(
        CancellationToken cancellationToken)
    {
        var cities = new[]
        {
            ("Nablus", "Palestine", "P400"),
            ("Ramallah", "Palestine", "P600"),
            ("Jerusalem", "Palestine", "P900"),
            ("Amman", "Jordan", "11118")
        };

        foreach (var (name, country, postOffice) in cities)
        {
            var exists = await context.Cities.AnyAsync(
                city => city.Name == name &&
                        city.Country == country,
                cancellationToken);

            if (exists)
            {
                continue;
            }

            context.Cities.Add(
                City.Create(name, country, postOffice));
        }

        await context.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Seeded development cities.");
    }

    private async Task SeedAmenitiesAsync(
        CancellationToken cancellationToken)
    {
        var amenities = new[]
        {
            "Wi-Fi",
            "Parking",
            "Pool",
            "Air Conditioning"
        };

        foreach (var name in amenities)
        {
            var exists = await context.Amenities.AnyAsync(
                amenity => amenity.Name == name,
                cancellationToken);

            if (exists)
            {
                continue;
            }

            context.Amenities.Add(
                Amenity.Create(name));
        }

        await context.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Seeded development amenities.");
    }

    private async Task SeedHotelsAsync(
        CancellationToken cancellationToken)
    {
        var ownerId = await context.Users
            .Where(user => user.Email == AdminEmail)
            .Select(user => user.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (ownerId == 0)
        {
            throw new InvalidOperationException(
                $"{AdminEmail} should have been seeded first.");
        }

        var cities = await context.Cities
            .Where(city =>
                city.Name == "Nablus" ||
                city.Name == "Ramallah" ||
                city.Name == "Amman")
            .ToDictionaryAsync(
                city => city.Name,
                cancellationToken);

        var amenities = await context.Amenities
            .ToDictionaryAsync(
                amenity => amenity.Name,
                cancellationToken);

        await EnsureHotelAsync(
            "Nablus Grand Hotel",
            "A comfortable hotel in the heart of Nablus.",
            cities["Nablus"],
            ownerId,
            amenities,
            cancellationToken);

        await EnsureHotelAsync(
            "Olive Garden Inn",
            "A quiet hotel near the old city.",
            cities["Nablus"],
            ownerId,
            amenities,
            cancellationToken);

        await EnsureHotelAsync(
            "Ramallah Palace",
            "A modern hotel close to the city centre.",
            cities["Ramallah"],
            ownerId,
            amenities,
            cancellationToken);

        await EnsureHotelAsync(
            "Amman Royal Hotel",
            "A comfortable hotel for visitors to Amman.",
            cities["Amman"],
            ownerId,
            amenities,
            cancellationToken);
    }

    private async Task EnsureHotelAsync(
        string name,
        string description,
        City city,
        int ownerId,
        Dictionary<string, Amenity> amenities,
        CancellationToken cancellationToken)
    {
        var hotel = await context.Hotels
            .FirstOrDefaultAsync(
                hotel => hotel.Name == name,
                cancellationToken);

        if (hotel is null)
        {
            hotel = Hotel.Create(
                city.Id,
                ownerId,
                name,
                description,
                4,
                HotelType.Luxury,
                $"{city.Name} City Centre",
                0,
                0);

            hotel.SetAmenities(
            [
                amenities["Wi-Fi"],
                amenities["Parking"],
                amenities["Air Conditioning"]
            ]);

            context.Hotels.Add(hotel);

            await context.SaveChangesAsync(cancellationToken);

            logger.LogInformation(
                "Seeded hotel {HotelName}",
                name);
        }

        await EnsureRoomTypesAsync(hotel, cancellationToken);
    }

    private async Task EnsureRoomTypesAsync(
        Hotel hotel,
        CancellationToken cancellationToken)
    {
        await EnsureRoomTypeAsync(
            hotel,
            "Standard",
            "A comfortable room with two beds.",
            75m,
            2,
            1,
            cancellationToken);

        await EnsureRoomTypeAsync(
            hotel,
            "Deluxe",
            "A larger room with extra space.",
            120m,
            2,
            2,
            cancellationToken);
    }

    private async Task EnsureRoomTypeAsync(
        Hotel hotel,
        string name,
        string description,
        decimal pricePerNight,
        int maxAdults,
        int maxChildren,
        CancellationToken cancellationToken)
    {
        var roomType = await context.RoomTypes
            .FirstOrDefaultAsync(
                type => type.HotelId == hotel.Id &&
                        type.Name == name,
                cancellationToken);

        if (roomType is null)
        {
            roomType = RoomType.Create(
                hotel.Id,
                name,
                description,
                pricePerNight,
                maxAdults,
                maxChildren);

            context.RoomTypes.Add(roomType);

            await context.SaveChangesAsync(cancellationToken);

            logger.LogInformation(
                "Seeded room type {RoomType} for {Hotel}",
                name,
                hotel.Name);
        }

        await EnsureRoomsAsync(
            hotel,
            roomType,
            cancellationToken);
    }

    private async Task EnsureRoomsAsync(
        Hotel hotel,
        RoomType roomType,
        CancellationToken cancellationToken)
    {
        var roomNumbers = roomType.Name == "Standard"
            ? new[] { "101", "102" }
            : new[] { "201", "202" };

        foreach (var number in roomNumbers)
        {
            var exists = await context.Rooms.AnyAsync(
                room => room.HotelId == hotel.Id &&
                        room.Number == number,
                cancellationToken);

            if (exists)
            {
                continue;
            }

            context.Rooms.Add(
                Room.Create(
                    hotel.Id,
                    roomType.Id,
                    number));
        }

        await context.SaveChangesAsync(cancellationToken);
    }
}