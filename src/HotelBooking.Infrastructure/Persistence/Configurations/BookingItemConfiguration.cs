using HotelBooking.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelBooking.Infrastructure.Persistence.Configurations;

public sealed class BookingItemConfiguration : IEntityTypeConfiguration<BookingItem>
{
    public void Configure(EntityTypeBuilder<BookingItem> builder)
    {
        builder.ToTable("BookingItems", table =>
            table.HasCheckConstraint("CK_BookingItems_Dates", "[CheckOut] > [CheckIn]"));

        builder.HasKey(item => item.Id);

        builder.Property(item => item.PricePerNight).HasPrecision(18, 2);

        builder.Ignore(item => item.Stay);

        builder.HasOne<Room>()
            .WithMany(room => room.BookingItems)
            .HasForeignKey(item => item.RoomId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasIndex(item => new { item.RoomId, item.CheckIn, item.CheckOut })
            .IncludeProperties(item => item.IsActive);
    }
}