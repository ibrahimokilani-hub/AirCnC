using HotelBooking.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelBooking.Infrastructure.Persistence.Configurations;

public sealed class HotelConfiguration : IEntityTypeConfiguration<Hotel>
{
    public void Configure(EntityTypeBuilder<Hotel> builder)
    {
        builder.ToTable("Hotels", table =>
            table.HasCheckConstraint(
                "CK_Hotels_StarRating",
                $"[StarRating] BETWEEN {Hotel.MinStars} AND {Hotel.MaxStars}"));

        builder.HasKey(hotel => hotel.Id);

        builder.Property(hotel => hotel.Name).IsRequired().HasMaxLength(Hotel.NameMaxLength);
        builder.Property(hotel => hotel.Description).IsRequired().HasMaxLength(Hotel.DescriptionMaxLength);
        builder.Property(hotel => hotel.Address).IsRequired().HasMaxLength(Hotel.AddressMaxLength);

        builder.Property(hotel => hotel.HotelType)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(hotel => hotel.Latitude).HasPrecision(9, 6);
        builder.Property(hotel => hotel.Longitude).HasPrecision(9, 6);

        builder.HasIndex(hotel => hotel.OwnerId);

        builder.HasOne(hotel => hotel.Owner)
            .WithMany()
            .HasForeignKey(hotel => hotel.OwnerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(hotel => hotel.City)
            .WithMany(city => city.Hotels)
            .HasForeignKey(hotel => hotel.CityId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(hotel => hotel.Name);
        builder.HasIndex(hotel => hotel.StarRating);
    }
}
