using HotelBooking.Contracts.Common;
using HotelBooking.Core.Domain.Common;
using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.Api.Controllers;

[ApiController]
[Produces("application/json")]
public abstract class ApiController : ControllerBase
{
    protected IActionResult OkData<T>(T data) => Ok(new ApiResponse<T>(data));
    protected IActionResult OkPaged<T>(PagedResult<T> page) => 
        Ok(new PagedResponse<T>(
            page.Items,
            new PageMeta(page.Page, page.PageSize, page.TotalCount, page.TotalPages)));
    
    protected IActionResult HandleFailure(Error error)
    {
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
        
        var body = error is ValidationError validationError
            ? new ErrorResponse(statusCode, error.Message, validationError.Errors)
            : new ErrorResponse(statusCode, error.Message);

        return StatusCode(statusCode, body);
    }
}