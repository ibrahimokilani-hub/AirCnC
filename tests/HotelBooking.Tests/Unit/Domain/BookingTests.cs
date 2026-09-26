using HotelBooking.Core.Domain.Entities;
using HotelBooking.Core.Domain.ValueObjects;

namespace HotelBooking.Tests.Unit.Domain;

public sealed class BookingTests
{
    private static readonly DateOnly CheckIn = new(2026, 6, 1);
    private static readonly DateTime NowUtc = new(2026, 6, 10, 9, 0, 0, DateTimeKind.Utc);

    private static Booking Booked(string? notes = null) =>
        Booking.Create(1, 1, Guid.NewGuid(), notes, NowUtc);

    private static DateRange Stay(int nights) => DateRange.Create(CheckIn, CheckIn.AddDays(nights));

    [Fact]
    public void AddItem_AddsPricePerNightTimesNights_ToTheTotal()
    {
        var booking = Booked();

        booking.AddItem(1, Stay(3), 2, 0, 100m);
        booking.AddItem(2, Stay(2), 1, 1, 50m);

        Assert.Equal(400m, booking.TotalPrice);
        Assert.Equal(2, booking.Items.Count);
    }

    [Fact]
    public void AddItem_WithoutAnAdult_Throws() =>
        Assert.Throws<ArgumentOutOfRangeException>(() => Booked().AddItem(1, Stay(1), 0, 2, 100m));

    [Theory]
    [InlineData("   ", null)]
    [InlineData(" late arrival ", "late arrival")]
    public void Create_BlankNotes_BecomeNull(string? notes, string? expected) =>
        Assert.Equal(expected, Booked(notes).Notes);

    [Fact]
    public void Create_WithoutAnIdempotencyKey_Throws() =>
        Assert.Throws<ArgumentException>(() => Booking.Create(1, 1, Guid.Empty, null, NowUtc));

    [Fact]
    public void WriteReview_BeforeTheStayIsOver_Fails()
    {
        var booking = Booked();
        booking.AddItem(1, Stay(3), 2, 0, 100m);

        var result = booking.WriteReview("Sara Haddad", 5, "Lovely.", CheckIn.AddDays(1), NowUtc);

        Assert.True(result.IsFailure);
        Assert.Equal("Booking.StayNotFinished", result.Error!.Code);
    }

    [Fact]
    public void WriteReview_OnceTheGuestHasCheckedOut_ReturnsTheReview()
    {
        var booking = Booked();
        booking.AddItem(1, Stay(3), 2, 0, 100m);

        var result = booking.WriteReview(" Sara Haddad ", 4, " Lovely. ", CheckIn.AddDays(3), NowUtc);

        Assert.True(result.IsSuccess);
        Assert.Equal("Sara Haddad", result.Value.ReviewerName);
        Assert.Equal("Lovely.", result.Value.Comment);
        Assert.Equal(4, result.Value.Rating);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(6)]
    public void WriteReview_RatingOutsideOneToFive_Throws(int rating)
    {
        var booking = Booked();
        booking.AddItem(1, Stay(1), 2, 0, 100m);

        Assert.Throws<ArgumentOutOfRangeException>(
            () => booking.WriteReview("Sara Haddad", rating, "Lovely.", CheckIn.AddDays(2), NowUtc));
    }
}