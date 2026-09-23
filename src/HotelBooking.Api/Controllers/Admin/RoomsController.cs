using HotelBooking.Contracts.Common;
using HotelBooking.Contracts.Rooms;
using HotelBooking.Core.Application.Abstractions.Messaging;
using HotelBooking.Core.Application.Features.Rooms.Commands.CreateRoom;
using HotelBooking.Core.Application.Features.Rooms.Commands.DeleteRoom;
using HotelBooking.Core.Application.Features.Rooms.Commands.UpdateRoom;
using HotelBooking.Core.Application.Features.Rooms.Queries.GetRoomsList;
using HotelBooking.Core.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.Api.Controllers.Admin;

[Route("api/v1/admin/hotels/{hotelId:int:min(1)}/rooms")]
[Tags("Admin · Rooms")]
public sealed class RoomsController(
    ICommandHandler<CreateRoomCommand, int> createRoom,
    ICommandHandler<UpdateRoomCommand> updateRoom,
    ICommandHandler<DeleteRoomCommand> deleteRoom,
    IQueryHandler<GetRoomsListQuery, PagedResult<RoomListItem>> getRoomsList)
    : ApiController
{
    /// <summary>The hotel's rooms grid. sortBy: number (default), roomType, createdAt.</summary>
    [Authorize(Roles = nameof(UserRole.Admin))]
    [HttpGet]
    [ProducesResponseType<PagedResponse<RoomListItem>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetList(int hotelId, [FromQuery] FilteredRequest request, CancellationToken cancellationToken)
    {
        var query = new GetRoomsListQuery(
            hotelId, request.Page, request.PageSize, request.Search, request.SortBy, request.SortDirection);

        var result = await getRoomsList.HandleAsync(query, cancellationToken);

        return result.IsFailure ? HandleFailure(result.Error!) : OkPaged(result.Value);
    }

    /// <summary>Adds a room to the hotel.</summary>
    /// <response code="409">The hotel already has a room with that number.</response>
    [Authorize(Roles = nameof(UserRole.Admin))]
    [Authorize(Roles = nameof(UserRole.Admin))]
    [HttpPost]
    [ProducesResponseType<ApiResponse<IdResponse>>(StatusCodes.Status201Created)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create(int hotelId, [FromBody] RoomRequest request, CancellationToken cancellationToken)
    {
        var result = await createRoom.HandleAsync(
            new CreateRoomCommand(hotelId, request.Number, request.RoomTypeId),
            cancellationToken);

        return result.IsFailure
            ? HandleFailure(result.Error!)
            : StatusCode(StatusCodes.Status201Created, new ApiResponse<IdResponse>(new IdResponse(result.Value)));
    }

    /// <summary>Changes a room's number or type.</summary>
    [Authorize(Roles = nameof(UserRole.Admin))]
    [HttpPut("{id:int:min(1)}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(int hotelId, int id, [FromBody] RoomRequest request, CancellationToken cancellationToken)
    {
        var result = await updateRoom.HandleAsync(
            new UpdateRoomCommand(hotelId, id, request.Number, request.RoomTypeId),
            cancellationToken);

        return result.IsFailure ? HandleFailure(result.Error!) : NoContent();
    }

    /// <summary>Deletes a room (soft delete).</summary>
    [Authorize(Roles = nameof(UserRole.Admin))]
    [HttpDelete("{id:int:min(1)}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int hotelId, int id, CancellationToken cancellationToken)
    {
        var result = await deleteRoom.HandleAsync(new DeleteRoomCommand(hotelId, id), cancellationToken);

        return result.IsFailure ? HandleFailure(result.Error!) : NoContent();
    }
}
