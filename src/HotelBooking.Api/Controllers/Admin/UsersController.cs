using HotelBooking.Contracts.Common;
using HotelBooking.Contracts.Users;
using HotelBooking.Contracts.Users.Responses;
using HotelBooking.Core.Application.Abstractions.Messaging;
using HotelBooking.Core.Application.Features.Users;
using HotelBooking.Core.Application.Features.Users.Queries;
using HotelBooking.Core.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.Api.Controllers.Admin;

[Route("api/v1/admin/users")]
[Tags("Admin · Users")]
public sealed class UsersController(
    IQueryHandler<GetUsersQuery, PagedResult<UserListItem>> getUsers)
    : ApiController
{
    [Authorize(Roles = nameof(UserRole.Admin))]
    [HttpGet]
    [ProducesResponseType<PagedResponse<UserListItem>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await getUsers.HandleAsync(new GetUsersQuery(page, pageSize), cancellationToken);

        return result.IsFailure ? HandleFailure(result.Error!) : OkPaged(result.Value);
    }
}