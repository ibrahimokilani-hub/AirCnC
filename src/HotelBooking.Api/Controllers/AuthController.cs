using HotelBooking.Contracts.Auth.Requests;
using HotelBooking.Contracts.Auth.Responses;
using HotelBooking.Contracts.Common;
using HotelBooking.Core.Application.Abstractions.Messaging;
using HotelBooking.Core.Application.Features.Auth.Login;
using HotelBooking.Core.Application.Features.Auth.Query.GetMe;
using HotelBooking.Core.Application.Features.Auth.Register;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.Api.Controllers;

[Route("api/v1/auth")]
[Tags("Auth")]
public sealed class AuthController(
    ICommandHandler<RegisterCommand, int> register,
    ICommandHandler<LoginCommand, AuthResponse> login,
    IQueryHandler<GetMeQuery, LoggedInUserResponse> getMe) : ApiController
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
    
    /// <summary>Logs in</summary>
    /// <response code="200">Logged in successfully</response>
    /// <response code="401">Invalid email or password</response>
    [HttpPost("login")]
    [ProducesResponseType<ApiResponse<IdResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var command = new LoginCommand(request.Email, request.Password);

        var result = await login.HandleAsync(command, cancellationToken);

        return result.IsFailure
            ? HandleFailure(result.Error!)
            : OkData(result.Value);
    }
    /// <summary>The logged-in user, read from the access token.</summary>
    /// <response code="200">The user and their role.</response>
    /// <response code="401">No token, or an invalid or expired one.</response>
    [Authorize]
    [HttpGet("me")]
    [ProducesResponseType<ApiResponse<LoggedInUserResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Me(CancellationToken cancellationToken)
    {
        var result = await getMe.HandleAsync(new GetMeQuery(), cancellationToken);

        return result.IsFailure ? HandleFailure(result.Error!) : OkData(result.Value);
    }
}