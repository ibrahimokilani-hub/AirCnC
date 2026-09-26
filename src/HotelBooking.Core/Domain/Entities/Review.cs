using HotelBooking.Core.Domain.Common;

namespace HotelBooking.Core.Domain.Entities;

public sealed class Review : Entity
{
    public const int MinRating = 1;
    public const int MaxRating = 5;
    public const int CommentMaxLength = 2000;

    // First name + space + last name; each name is capped at 100, so 201 covers the longest.
    public const int ReviewerNameMaxLength = 201;

    private Review(int hotelId, int userId, int bookingId, string reviewerName, int rating, string comment, DateTime createdAtUtc)
    {
        HotelId = hotelId;
        UserId = userId;
        BookingId = bookingId;
        ReviewerName = reviewerName;
        Rating = rating;
        Comment = comment;
        CreatedAtUtc = createdAtUtc;
    }

    public int HotelId { get; private set; }

    public int UserId { get; private set; }

    public int BookingId { get; private set; }

    /// <summary>
    /// The author's display name, snapshotted when the review is written. A review is
    /// history: it must read the same even if the account is later renamed or deleted,
    /// so the name is stored here rather than joined from the User on every read.
    /// </summary>
    public string ReviewerName { get; private set; }

    public int Rating { get; private set; }

    public string Comment { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }

    internal static Review Create(
        int hotelId,
        int userId,
        int bookingId,
        string reviewerName,
        int rating,
        string comment,
        DateTime createdAtUtc)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(rating, MinRating);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(rating, MaxRating);
        ArgumentException.ThrowIfNullOrWhiteSpace(comment);
        ArgumentException.ThrowIfNullOrWhiteSpace(reviewerName);

        return new Review(hotelId, userId, bookingId, reviewerName.Trim(), rating, comment.Trim(), createdAtUtc);
    }
}
