using HotelBooking.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelBooking.Infrastructure.Persistence.Configurations;

public sealed class RoomConfiguration : IEntityTypeConfiguration<Room>
{
    public void Configure(EntityTypeBuilder<Room> builder)
    {
        builder.ToTable("Rooms");

        builder.HasKey(room => room.Id);

        builder.Property(room => room.Number).IsRequired().HasMaxLength(Room.NumberMaxLength);

        builder.HasOne<Hotel>()
            .WithMany(hotel => hotel.Rooms)
            .HasForeignKey(room => room.HotelId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(room => room.RoomType)
            .WithMany()
            .HasForeignKey(room => room.RoomTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(room => new { room.HotelId, room.Number })
            .IsUnique()
            .HasFilter("[IsDeleted] = 0");
    }
}