using HotelBooking.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelBooking.Infrastructure.Persistence.Configurations;

public class CityConfiguration : IEntityTypeConfiguration<City>
{
    public void Configure(EntityTypeBuilder<City> builder)
    {
        builder.ToTable("Cities");

        builder.HasKey(city => city.Id);

        builder.Property(city => city.Name)
            .IsRequired()
            .HasMaxLength(70);
        
        builder.Property(city => city.Country
            )
            .IsRequired()
            .HasMaxLength(40);
        
        builder.Property(city => city.PostOffice)
            .IsRequired()
            .HasMaxLength(15);
        
        builder.HasIndex(city => new { city.Name, city.Country })
            .IsUnique();
    }
}