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
    
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}