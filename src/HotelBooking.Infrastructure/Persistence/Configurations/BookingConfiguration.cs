using HotelBooking.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelBooking.Infrastructure.Persistence.Configurations;

public sealed class BookingConfiguration : IEntityTypeConfiguration<Booking>
{
    public void Configure(EntityTypeBuilder<Booking> builder)
    {
        builder.ToTable("Bookings");

        builder.HasKey(booking => booking.Id);


        builder.Ignore(booking => booking.ConfirmationNumber);
        builder.HasIndex(booking => new { booking.UserId, booking.IdempotencyKey }).IsUnique();

        builder.Property(booking => booking.Status).HasConversion<string>().HasMaxLength(20);
        builder.Property(booking => booking.Notes).HasMaxLength(1000);
        builder.Property(booking => booking.TotalPrice).HasPrecision(18, 2);

        builder.HasIndex(booking => new { booking.UserId, booking.CreatedAtUtc });

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(booking => booking.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Hotel>()
            .WithMany()
            .HasForeignKey(booking => booking.HotelId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(booking => booking.Items)
            .WithOne()
            .HasForeignKey(item => item.BookingId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
