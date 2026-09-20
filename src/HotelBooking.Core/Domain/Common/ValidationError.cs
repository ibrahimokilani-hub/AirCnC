namespace HotelBooking.Core.Domain.Common;

public class ValidationError : Error
{
    public ValidationError(IReadOnlyDictionary<string, string[]> errors)
        : base(ErrorType.Validation, "Validation.Failed", "One or more validation errors occurred.")
    {
        Errors = errors;
    }

    public IReadOnlyDictionary<string, string[]> Errors { get; }
}