namespace HotelBooking.Core.Domain.Common;

public class Result
{
    private Result(Error? error)
    {
        Error = error;
    }

    public Error? Error { get; }

    public bool IsSuccess => Error is null;

    public bool IsFailure => Error is not null;

    public static Result Success() => new(null);

    public static Result Failure(Error error) => new(error);
}

public class Result<TValue>
{
    private readonly TValue? _value;

    private Result(TValue? value, Error? error)
    {
        _value = value;
        Error = error;
    }

    public Error? Error { get; }

    public bool IsSuccess => Error is null;

    public bool IsFailure => Error is not null;

    /// <summary>Throws when the result is a failure. Check IsSuccess first.</summary>
    public TValue Value => IsSuccess
        ? _value!
        : throw new InvalidOperationException("Cannot read the value of a failed result.");

    public static Result<TValue> Success(TValue value) => new(value, null);

    public static Result<TValue> Failure(Error error) => new(default, error);
}