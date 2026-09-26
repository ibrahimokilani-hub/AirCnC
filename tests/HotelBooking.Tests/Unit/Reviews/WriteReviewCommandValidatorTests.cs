using HotelBooking.Core.Application.Features.Reviews.Commands.WriteReview;
using HotelBooking.Core.Domain.Entities;

namespace HotelBooking.Tests.Unit.Reviews;

public sealed class WriteReviewCommandValidatorTests
{
    private readonly WriteReviewCommandValidator _validator = new();

    private static WriteReviewCommand Valid() => new(1, 5, "Clean rooms and a kind welcome.");

    private string[] FailedProperties(WriteReviewCommand command) =>
        _validator.Validate(command).Errors.Select(error => error.PropertyName).Distinct().ToArray();

    [Fact]
    public void Validate_ValidCommand_HasNoErrors() =>
        Assert.True(_validator.Validate(Valid()).IsValid);

    [Theory]
    [InlineData(0)]
    [InlineData(6)]
    public void Validate_RatingOutsideOneToFive_HasRatingError(int rating) =>
        Assert.Contains(nameof(WriteReviewCommand.Rating), FailedProperties(Valid() with { Rating = rating }));

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Validate_BlankComment_HasCommentError(string comment) =>
        Assert.Contains(nameof(WriteReviewCommand.Comment), FailedProperties(Valid() with { Comment = comment }));

    [Fact]
    public void Validate_CommentOverTheLimit_HasCommentError()
    {
        var command = Valid() with { Comment = new string('a', Review.CommentMaxLength + 1) };

        Assert.Contains(nameof(WriteReviewCommand.Comment), FailedProperties(command));
    }
}