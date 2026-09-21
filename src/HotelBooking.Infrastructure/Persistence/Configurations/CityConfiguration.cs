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
            .HasMaxLength(City.NameMaxLength);
        
        builder.Property(city => city.Country
            )
            .IsRequired()
            .HasMaxLength(City.CountryMaxLength);
        
        builder.Property(city => city.PostOffice)
            .IsRequired()
            .HasMaxLength(City.PostOfficeMaxLength);
        
        builder.HasIndex(city => new { city.Name, city.Country })
            .IsUnique()
            .HasFilter("[IsDeleted] = 0");
    }
}