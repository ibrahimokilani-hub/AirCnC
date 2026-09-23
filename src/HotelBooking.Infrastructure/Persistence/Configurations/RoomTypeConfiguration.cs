using HotelBooking.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelBooking.Infrastructure.Persistence.Configurations;

public sealed class RoomTypeConfiguration : IEntityTypeConfiguration<RoomType>
{
    public void Configure(EntityTypeBuilder<RoomType> builder)
    {
        builder.ToTable("RoomTypes", table =>
        {
            table.HasCheckConstraint("CK_RoomTypes_PricePerNight", "[PricePerNight] > 0");
            table.HasCheckConstraint("CK_RoomTypes_MaxAdults", "[MaxAdults] >= 1");
            table.HasCheckConstraint("CK_RoomTypes_MaxChildren", "[MaxChildren] >= 0");
        });

        builder.HasKey(roomType => roomType.Id);

        builder.Property(roomType => roomType.Name).IsRequired().HasMaxLength(RoomType.NameMaxLength);
        builder.Property(roomType => roomType.Description).IsRequired().HasMaxLength(RoomType.DescriptionMaxLength);

        builder.Property(roomType => roomType.PricePerNight).HasPrecision(18, 2);

        builder.HasOne<Hotel>()
            .WithMany(hotel => hotel.RoomTypes)
            .HasForeignKey(roomType => roomType.HotelId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}