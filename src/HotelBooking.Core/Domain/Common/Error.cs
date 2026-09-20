namespace HotelBooking.Core.Domain.Common;

public enum ErrorType
{
    Validation,     // 400
    Unauthorized,   // 401
    Forbidden,      // 403
    NotFound,       // 404
    Conflict        // 409
}

/// <summary>
/// An expected failure: something a correct client can legitimately run into.
/// </summary>
/// <param name="Type">Decides the HTTP status.</param>
/// <param name="Code">A stable machine-readable key, e.g. "City.NotFound".</param>
/// <param name="Message">A human-readable sentence, safe to show a user.</param>
public record Error(ErrorType Type, string Code, string Message)
{
    public static Error NotFound(string code, string message) =>
        new(ErrorType.NotFound, code, message);

    public static Error Conflict(string code, string message) =>
        new(ErrorType.Conflict, code, message);

    public static Error Unauthorized(string code, string message) =>
        new(ErrorType.Unauthorized, code, message);

    public static Error Forbidden(string code, string message) =>
        new(ErrorType.Forbidden, code, message);
}

public sealed record ValidationError(IReadOnlyDictionary<string, string[]> Errors)
    : Error(ErrorType.Validation, "Validation.Failed", "One or more validation errors occurred.")
{
    /// <summary>A single field error, for checks a validator cannot do alone.</summary>
    public static ValidationError Single(string field, string message) =>
        new(new Dictionary<string, string[]> { [field] = [message] });
}