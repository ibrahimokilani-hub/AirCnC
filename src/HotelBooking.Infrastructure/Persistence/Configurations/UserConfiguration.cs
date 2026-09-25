using HotelBooking.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelBooking.Infrastructure.Persistence.Configurations;

public sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(user => user.Id);

        builder.Property(user => user.Email).HasMaxLength(User.EmailMaxLength).IsRequired();
        builder.Property(user => user.FirstName).HasMaxLength(User.NameMaxLength).IsRequired();
        builder.Property(user => user.LastName).HasMaxLength(User.NameMaxLength).IsRequired();
        builder.Property(user => user.Phone).HasMaxLength(User.PhoneMaxLength).IsRequired();

        builder.Property(user => user.PasswordHash)
            .HasMaxLength(User.PasswordHashMaxLength)
            .IsUnicode(false)
            .IsRequired();

        builder.Property(user => user.Role).HasConversion<string>().HasMaxLength(20);


        builder.HasIndex(user => user.Email).IsUnique();
    }
}
