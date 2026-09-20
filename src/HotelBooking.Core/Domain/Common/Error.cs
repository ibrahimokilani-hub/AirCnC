namespace HotelBooking.Core.Domain.Common;

public enum ErrorType
{
    Validation,  
    Unauthorized,   
    Forbidden,      
    NotFound,      
    Conflict        
}

public class Error
{
    public Error(ErrorType type, string code, string message)
    {
        Type = type;
        Code = code;
        Message = message;
    }

    public ErrorType Type { get; }

    public string Code { get; }

    public string Message { get; }

    public static Error NotFound(string code, string message) =>
        new(ErrorType.NotFound, code, message);

    public static Error Conflict(string code, string message) =>
        new(ErrorType.Conflict, code, message);

    public static Error Unauthorized(string code, string message) =>
        new(ErrorType.Unauthorized, code, message);

    public static Error Forbidden(string code, string message) =>
        new(ErrorType.Forbidden, code, message);
}