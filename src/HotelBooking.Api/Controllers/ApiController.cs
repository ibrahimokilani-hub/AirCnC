using HotelBooking.Core.Domain.Common;
using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.Api.Controllers;

[ApiController]
public abstract class ApiController : ControllerBase
{
    protected IActionResult HandleFailure(Error error)
    {
        if (error is ValidationError validationError)
        {
            var details = new ValidationProblemDetails(
                validationError.Errors.ToDictionary(entry => entry.Key, entry => entry.Value))
            {
                Status = StatusCodes.Status400BadRequest,
                Title = error.Message
            };

            return BadRequest(details);
        }

        var statusCode = error.Type switch
        {
            ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
            ErrorType.Forbidden => StatusCodes.Status403Forbidden,
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            _ => throw new ArgumentOutOfRangeException(
                nameof(error), error.Type, "Unmapped error type.")
        };

        return Problem(statusCode: statusCode, title: error.Code, detail: error.Message);
    }
}