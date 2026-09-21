using FluentValidation.Results;
using HotelBooking.Core.Application.Common.Validation;
using HotelBooking.Core.Domain.Common;

namespace HotelBooking.Tests.Unit.Common;

public sealed class ValidationExtensionsTests
{
    [Fact]
    public void ToValidationError_GroupsFailuresByProperty()
    {
        var result = new ValidationResult(
        [
            new ValidationFailure("Name", "City name is required."),
            new ValidationFailure("Name", "The length must be 70 characters or fewer."),
            new ValidationFailure("Country", "Country name is required.")
        ]);

        var error = result.ToValidationError();

        Assert.Equal(ErrorType.Validation, error.Type);
        Assert.Equal(2, error.Errors.Count);
        Assert.Equal(2, error.Errors["Name"].Length);
        Assert.Equal(["Country name is required."], error.Errors["Country"]);
    }
}