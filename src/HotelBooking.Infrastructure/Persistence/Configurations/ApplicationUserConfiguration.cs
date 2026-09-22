using HotelBooking.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelBooking.Infrastructure.Persistence.Configurations;

public sealed class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.Property(user => user.FirstName)
            .IsRequired()
            .HasMaxLength(ApplicationUser.NameMaxLength);
        
        builder.Property(user => user.LastName)
            .IsRequired()
            .HasMaxLength(ApplicationUser.NameMaxLength);
    }
}