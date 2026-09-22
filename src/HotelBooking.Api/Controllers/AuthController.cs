using HotelBooking.Contracts.Auth.Requests;
using HotelBooking.Contracts.Common;
using HotelBooking.Core.Application.Abstractions.Messaging;
using HotelBooking.Core.Application.Features.Auth.Register;
using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.Api.Controllers;

[Route("api/v1/auth")]
[Tags("Auth")]
public sealed class AuthController(ICommandHandler<RegisterCommand, int> register) : ApiController
{
    /// <summary>Creates a guest account</summary>
    /// <response code="201">The account was created. "data" holds the user id</response>
    /// <response code="400">Invalid email, weak password, or a missing name</response>
    /// <response code="409">An account with this email already exists</response>
    [HttpPost("register")]
    [ProducesResponseType<ApiResponse<IdResponse>>(StatusCodes.Status201Created)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken cancellationToken)
    {
        var command = new RegisterCommand(request.Email, request.Password, request.FirstName, request.LastName);

        var result = await register.HandleAsync(command, cancellationToken);

        return result.IsFailure
            ? HandleFailure(result.Error!)
            : StatusCode(StatusCodes.Status201Created, new ApiResponse<IdResponse>(new IdResponse(result.Value)));
    }
}