using HotelBooking.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelBooking.Infrastructure.Persistence.Configurations;

public sealed class DiscountConfiguration : IEntityTypeConfiguration<Discount>
{
    public void Configure(EntityTypeBuilder<Discount> builder)
    {
        builder.ToTable("Discounts", table =>
        {
            table.HasCheckConstraint(
                "CK_Discounts_Value",
                """
                ([DiscountType] = 'Percentage' AND [Value] >= 1 AND [Value] <= 90)
                OR
                ([DiscountType] = 'FixedAmount' AND [Value] > 0)
                """);

            table.HasCheckConstraint(
                "CK_Discounts_Dates",
                "[EndsAt] >= [StartsAt]");
        });

        builder.HasKey(discount => discount.Id);

        builder.Property(discount => discount.DiscountType)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(discount => discount.Value)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.HasOne<Hotel>()
            .WithMany(hotel => hotel.Discounts)
            .HasForeignKey(discount => discount.HotelId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(discount => new
        {
            discount.StartsAt,
            discount.EndsAt
        });
    }
}