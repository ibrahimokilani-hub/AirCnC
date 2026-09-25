using HotelBooking.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Core.Application.Abstractions;

public interface IAppDbContext
{
    DbSet<City> Cities { get; }
    DbSet<User> Users { get; }
    DbSet<Hotel> Hotels { get; }
    DbSet<RoomType> RoomTypes { get; }
    DbSet<Room> Rooms { get; }
    DbSet<Amenity> Amenities { get; }
    DbSet<BookingItem> BookingItems { get; }
    DbSet<Booking> Bookings { get; }
    DbSet<NearbyAttraction> NearbyAttractions { get; }
    DbSet<Discount> Discounts { get; }
    
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}