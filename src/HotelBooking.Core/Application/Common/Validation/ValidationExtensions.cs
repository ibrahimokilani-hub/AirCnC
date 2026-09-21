using FluentValidation.Results;
using HotelBooking.Core.Domain.Common;

namespace HotelBooking.Core.Application.Common.Validation;

public static class ValidationExtensions
{
    public static ValidationError ToValidationError(this ValidationResult result) =>
        new(result.Errors
            .GroupBy(failure => failure.PropertyName)
            .ToDictionary(
                group => group.Key,
                group => group.Select(failure => failure.ErrorMessage).ToArray()));
}