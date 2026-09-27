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

    /// <summary>
    /// Real landmarks grouped by city, used to seed the standalone attraction catalog.
    /// They are not tied to any hotel — the grouping is only for readable coordinates.
    /// </summary>
    private static readonly Dictionary<string, (string Name, string Category, decimal Latitude, decimal Longitude)[]> Attractions =
        new()
        {
            ["Nablus"] =
            [
                ("Old City of Nablus", "Culture", 32.2205m, 35.2600m),
                ("Jacob's Well", "Landmark", 32.2098m, 35.2831m),
                ("Mount Gerizim", "Nature", 32.2000m, 35.2730m)
            ],
            ["Ramallah"] =
            [
                ("Al-Manara Square", "Landmark", 31.9046m, 35.2040m),
                ("Yasser Arafat Museum", "Museum", 31.9059m, 35.1970m),
                ("Ramallah Cultural Palace", "Culture", 31.8944m, 35.2028m)
            ],
            ["Amman"] =
            [
                ("Roman Theatre", "Landmark", 31.9515m, 35.9396m),
                ("Amman Citadel", "Culture", 31.9548m, 35.9344m),
                ("Rainbow Street", "Shopping", 31.9500m, 35.9270m)
            ]
        };

    /// <summary>
    /// The deals the home page ranks, keyed by hotel name: the deal's name, how it's
    /// applied, its value, and how many days it runs from today. Nablus Grand Hotel is
    /// deliberately absent — a featured-deals query that returns every hotel proves
    /// nothing. Ramallah Palace uses a FixedAmount so both branches of the query
    /// (and Discount.ApplyTo) are exercised.
    /// </summary>
    private static readonly Dictionary<string, (string Name, DiscountType Type, decimal Value, int Days)> Discounts =
        new()
        {
            ["Olive Garden Inn"] = ("Autumn Getaway", DiscountType.Percentage, 35m, 45),
            ["Amman Royal Hotel"] = ("City Break", DiscountType.Percentage, 25m, 30),
            ["Ramallah Palace"] = ("15 Off Your Stay", DiscountType.FixedAmount, 15m, 14)
        };

    /// <summary>
    /// Free Unsplash photos that give the dev data real pictures. Stored as plain URLs because
    /// ImageUrl is just a string today; Phase 2 swaps these for Cloudinary uploads. Keyed by
    /// hotel name: one cover (Hotel.HotelImageUrl) plus a couple of gallery shots.
    /// </summary>
    private static readonly Dictionary<string, (string Cover, string[] Gallery)> HotelImagery =
        new()
        {
            ["Nablus Grand Hotel"] = (
                "photo-1566073771259-6a8506099945",
                ["photo-1551882547-ff40c63fe5fa", "photo-1564501049412-61c2a3083791"]),
            ["Olive Garden Inn"] = (
                "photo-1571003123894-1f0594d2b5d9",
                ["photo-1582719478250-c89cae4dc85b", "photo-1520250497591-112f2f40a3f4"]),
            ["Ramallah Palace"] = (
                "photo-1542314831-068cd1dbfeeb",
                ["photo-1455587734955-081b22074882", "photo-1445019980597-93fa8acb246c"]),
            ["Amman Royal Hotel"] = (
                "photo-1611892440504-42a792e24d32",
                ["photo-1566073771259-6a8506099945", "photo-1551882547-ff40c63fe5fa"])
        };

    /// <summary>Gallery photos per room-type name. Every hotel's Standard/Deluxe reuse these.</summary>
    private static readonly Dictionary<string, string[]> RoomImagery =
        new()
        {
            ["Standard"] = ["photo-1618773928121-c32242e63f39", "photo-1590490360182-c33d57733427"],
            ["Deluxe"] = ["photo-1560448204-e02f11c3d0e2", "photo-1522708323590-d24dbb6b0267"]
        };

    /// <summary>Unsplash serves any size from one id; these params keep the files web-friendly.</summary>
    private static string ImageUrl(string photoId) =>
        $"https://images.unsplash.com/{photoId}?auto=format&fit=crop&w=1200&q=80";

    public async Task SeedAsync(
        CancellationToken cancellationToken = default)
    {
        await SeedUsersAsync(cancellationToken);
        await SeedCitiesAsync(cancellationToken);
        await SeedAmenitiesAsync(cancellationToken);
        await SeedHotelsAsync(cancellationToken);
        await SeedNearbyAttractionsAsync(cancellationToken);
    }

    /// <summary>
    /// Hard-deletes every row while leaving the schema and migration history in place, so a
    /// following SeedAsync starts from a clean database. Children go before parents, so the
    /// Restrict foreign keys never block, and IgnoreQueryFilters means even soft-deleted rows
    /// go. Uses ExecuteDelete (one DELETE per table, no entities loaded). Dev only — the
    /// --reset startup path is the only caller.
    /// </summary>
    public async Task ClearAsync(
        CancellationToken cancellationToken = default)
    {
        // The Hotel<->Amenity join has no entity of its own, so it is cleared with raw SQL.
        await context.Database.ExecuteSqlRawAsync(
            "DELETE FROM [HotelAmenities];",
            cancellationToken);

        await context.BookingItems.IgnoreQueryFilters().ExecuteDeleteAsync(cancellationToken);
        await context.Reviews.IgnoreQueryFilters().ExecuteDeleteAsync(cancellationToken);
        await context.Bookings.IgnoreQueryFilters().ExecuteDeleteAsync(cancellationToken);
        await context.RefreshTokens.IgnoreQueryFilters().ExecuteDeleteAsync(cancellationToken);
        await context.RoomTypeImages.IgnoreQueryFilters().ExecuteDeleteAsync(cancellationToken);
        await context.HotelImages.IgnoreQueryFilters().ExecuteDeleteAsync(cancellationToken);
        await context.Discounts.IgnoreQueryFilters().ExecuteDeleteAsync(cancellationToken);
        await context.Rooms.IgnoreQueryFilters().ExecuteDeleteAsync(cancellationToken);
        await context.RoomTypes.IgnoreQueryFilters().ExecuteDeleteAsync(cancellationToken);
        await context.NearbyAttractions.IgnoreQueryFilters().ExecuteDeleteAsync(cancellationToken);
        await context.Hotels.IgnoreQueryFilters().ExecuteDeleteAsync(cancellationToken);
        await context.Amenities.IgnoreQueryFilters().ExecuteDeleteAsync(cancellationToken);
        await context.Cities.IgnoreQueryFilters().ExecuteDeleteAsync(cancellationToken);
        await context.Users.IgnoreQueryFilters().ExecuteDeleteAsync(cancellationToken);

        logger.LogInformation("Cleared all data from the database.");
    }

    private async Task SeedUsersAsync(
        CancellationToken cancellationToken)
    {
        await EnsureUserAsync(
            AdminEmail,
            "Ada",
            "Admin",
            "+970 599 000 001",
            UserRole.Admin,
            cancellationToken);

        await EnsureUserAsync(
            GuestEmail,
            "Gus",
            "Guest",
            "+970 599 000 002",
            UserRole.User,
            cancellationToken);
    }

    private async Task EnsureUserAsync(
        string email,
        string firstName,
        string lastName,
        string phone,
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
            phone,
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
            32.2226m,
            35.2605m,
            amenities,
            cancellationToken);

        await EnsureHotelAsync(
            "Olive Garden Inn",
            "A quiet hotel near the old city.",
            cities["Nablus"],
            ownerId,
            32.2190m,
            35.2580m,
            amenities,
            cancellationToken);

        await EnsureHotelAsync(
            "Ramallah Palace",
            "A modern hotel close to the city centre.",
            cities["Ramallah"],
            ownerId,
            31.9070m,
            35.2015m,
            amenities,
            cancellationToken);

        await EnsureHotelAsync(
            "Amman Royal Hotel",
            "A comfortable hotel for visitors to Amman.",
            cities["Amman"],
            ownerId,
            31.9525m,
            35.9310m,
            amenities,
            cancellationToken);
    }

    private async Task EnsureHotelAsync(
        string name,
        string description,
        City city,
        int ownerId,
        decimal latitude,
        decimal longitude,
        Dictionary<string, Amenity> amenities,
        CancellationToken cancellationToken)
    {
        var hotel = await context.Hotels
            .FirstOrDefaultAsync(
                hotel => hotel.Name == name,
                cancellationToken);

        if (hotel is null)
        {
            var imagery = HotelImagery.GetValueOrDefault(name);
            var hotelType = Random.Shared.Next(3) switch
            {
                0 => HotelType.Luxury,
                1 => HotelType.Boutique,
                _ => HotelType.Budget
            };
            
            hotel = Hotel.Create(
                city.Id,
                ownerId,
                name,
                description,
                4,
                hotelType,
                $"{city.Name} City Centre",
                latitude,
                longitude,
                imagery.Cover is null ? null : ImageUrl(imagery.Cover));

            hotel.SetAmenities(
            [
                amenities["Wi-Fi"],
                amenities["Parking"],
                amenities["Air Conditioning"]
            ]);

            AddDiscount(hotel, name);

            context.Hotels.Add(hotel);

            // Save first so the hotel has an Id: HotelImage.Create rejects a zero HotelId, and
            // the gallery rows carry that Id as their foreign key.
            await context.SaveChangesAsync(cancellationToken);

            if (imagery.Gallery is not null)
            {
                foreach (var photoId in imagery.Gallery)
                {
                    hotel.AddImage(ImageUrl(photoId));
                }

                await context.SaveChangesAsync(cancellationToken);
            }

            logger.LogInformation(
                "Seeded hotel {HotelName}",
                name);
        }

        await EnsureRoomTypesAsync(hotel, cancellationToken);
    }

    /// <summary>
    /// Fills the standalone attraction catalog. Additive and idempotent: skipped once any
    /// attraction exists. No hotel involved — these rows stand on their own.
    /// </summary>
    private async Task SeedNearbyAttractionsAsync(
        CancellationToken cancellationToken)
    {
        if (await context.NearbyAttractions.AnyAsync(cancellationToken))
        {
            return;
        }

        foreach (var landmarks in Attractions.Values)
        {
            foreach (var (name, category, latitude, longitude) in landmarks)
            {
                context.NearbyAttractions.Add(
                    NearbyAttraction.Create(name, category, latitude, longitude));
            }
        }

        await context.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Seeded standalone attraction catalog.");
    }

    /// <summary>
    /// The deal goes on the aggregate before it is saved, like the map pins, so EF inserts
    /// it with the hotel. A hotel with no entry runs no deal. Not static, unlike
    /// AddAttractions: a deal is only a deal *today*, so it reads the injected clock.
    /// </summary>
    private void AddDiscount(
        Hotel hotel,
        string hotelName)
    {
        if (!Discounts.TryGetValue(hotelName, out var deal))
        {
            return;
        }

        var today = DateOnly.FromDateTime(
            timeProvider.GetUtcNow().UtcDateTime);

        hotel.AddDiscount(
            deal.Name,
            deal.Type,
            deal.Value,
            today,
            today.AddDays(deal.Days),
            isActive: true);
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

            // Save first: RoomTypeImage.Create needs the generated RoomType Id.
            await context.SaveChangesAsync(cancellationToken);

            if (RoomImagery.TryGetValue(name, out var photoIds))
            {
                foreach (var photoId in photoIds)
                {
                    roomType.AddImage(ImageUrl(photoId));
                }

                await context.SaveChangesAsync(cancellationToken);
            }

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