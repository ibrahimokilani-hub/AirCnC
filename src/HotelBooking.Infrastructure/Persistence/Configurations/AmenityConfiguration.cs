using HotelBooking.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelBooking.Infrastructure.Persistence.Configurations;

public sealed class AmenityConfiguration : IEntityTypeConfiguration<Amenity>
{
    public void Configure(EntityTypeBuilder<Amenity> builder)
    {
        builder.ToTable("Amenities");

        builder.HasKey(amenity => amenity.Id);

        builder.Property(amenity => amenity.Name).IsRequired().HasMaxLength(Amenity.NameMaxLength);

        builder.HasIndex(amenity => amenity.Name)
            .IsUnique()
            .HasFilter("[IsDeleted] = 0");
    }
}