using HotelBooking.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelBooking.Infrastructure.Persistence.Configurations;

public sealed class ReviewConfiguration : IEntityTypeConfiguration<Review>
{
    public void Configure(EntityTypeBuilder<Review> builder)
    {
        builder.ToTable("Reviews", table =>
            table.HasCheckConstraint("CK_Reviews_Rating", $"[Rating] BETWEEN {Review.MinRating} AND {Review.MaxRating}"));

        builder.HasKey(review => review.Id);

        builder.Property(review => review.Comment).HasMaxLength(Review.CommentMaxLength).IsRequired();

        builder.Property(review => review.ReviewerName).HasMaxLength(Review.ReviewerNameMaxLength).IsRequired();

        builder.HasIndex(review => review.BookingId).IsUnique();
        
        builder.HasOne<Hotel>()
            .WithMany()
            .HasForeignKey(review => review.HotelId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Booking>()
            .WithMany()
            .HasForeignKey(review => review.BookingId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(review => review.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}