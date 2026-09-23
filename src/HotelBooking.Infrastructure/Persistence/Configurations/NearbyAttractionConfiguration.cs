using HotelBooking.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelBooking.Infrastructure.Persistence.Configurations;

public sealed class NearbyAttractionConfiguration : IEntityTypeConfiguration<NearbyAttraction>
{
    public void Configure(EntityTypeBuilder<NearbyAttraction> builder)
    {
        builder.ToTable("NearbyAttractions");

        builder.HasKey(attraction => attraction.Id);

        builder.Property(attraction => attraction.Name).IsRequired().HasMaxLength(NearbyAttraction.NameMaxLength);
        builder.Property(attraction => attraction.Category).IsRequired().HasMaxLength(NearbyAttraction.CategoryMaxLength);
        builder.Property(attraction => attraction.Latitude).HasPrecision(9, 6);
        builder.Property(attraction => attraction.Longitude).HasPrecision(9, 6);

        builder.HasOne<Hotel>()
            .WithMany(hotel => hotel.NearbyAttractions)
            .HasForeignKey(attraction => attraction.HotelId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}