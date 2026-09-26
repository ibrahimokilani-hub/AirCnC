using HotelBooking.Contracts.Common;
using HotelBooking.Contracts.RoomTypes;
using HotelBooking.Contracts.RoomTypes.requests;
using HotelBooking.Contracts.RoomTypes.Responses;
using HotelBooking.Core.Application.Abstractions.Messaging;
using HotelBooking.Core.Application.Features.RoomTypes.Commands.AddRoomTypeImage;
using HotelBooking.Core.Application.Features.RoomTypes.Commands.CreateRoomType;
using HotelBooking.Core.Application.Features.RoomTypes.Commands.DeleteRoomType;
using HotelBooking.Core.Application.Features.RoomTypes.Commands.RemoveRoomTypeImage;
using HotelBooking.Core.Application.Features.RoomTypes.Commands.UpdateRoomType;
using HotelBooking.Core.Application.Features.RoomTypes.Queries.GetRoomTypes;
using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.Api.Controllers.Admin;

/// <summary>Room types belong to one hotel, and the URL says which.</summary>
[Route("api/v1/admin/hotels/{hotelId:int:min(1)}/room-types")]
[Tags("Admin · Room types")]
public sealed class RoomTypesController(
    ICommandHandler<CreateRoomTypeCommand, int> createRoomType,
    ICommandHandler<UpdateRoomTypeCommand> updateRoomType,
    ICommandHandler<DeleteRoomTypeCommand> deleteRoomType,
    IQueryHandler<GetRoomTypesQuery, IReadOnlyList<RoomTypeResponse>> getRoomTypes,
    ICommandHandler<AddRoomTypeImageCommand, int> addRoomTypeImage,
    ICommandHandler<RemoveRoomTypeImageCommand> removeRoomTypeImage)
    : ApiController
{
    /// <summary>Every room type of the hotel, cheapest first.</summary>
    /// <response code="200">{ "data": [ ... ] }</response>
    /// <response code="404">No such hotel.</response>
    [HttpGet]
    [ProducesResponseType<ApiResponse<IReadOnlyList<RoomTypeResponse>>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAll(int hotelId, CancellationToken cancellationToken)
    {
        var result = await getRoomTypes.HandleAsync(new GetRoomTypesQuery(hotelId), cancellationToken);

        return result.IsFailure ? HandleFailure(result.Error!) : OkData(result.Value);
    }

    /// <summary>Adds a room type to the hotel.</summary>
    [HttpPost]
    [ProducesResponseType<ApiResponse<IdResponse>>(StatusCodes.Status201Created)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Create(int hotelId, [FromBody] RoomTypeRequest request, CancellationToken cancellationToken)
    {
        var command = new CreateRoomTypeCommand(
            hotelId, request.Name, request.Description, request.PricePerNight, request.MaxAdults, request.MaxChildren);

        var result = await createRoomType.HandleAsync(command, cancellationToken);

        return result.IsFailure
            ? HandleFailure(result.Error!)
            : StatusCode(StatusCodes.Status201Created, new ApiResponse<IdResponse>(new IdResponse(result.Value)));
    }

    /// <summary>Replaces a room type's fields.</summary>
    [HttpPut("{id:int:min(1)}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int hotelId, int id, [FromBody] RoomTypeRequest request, CancellationToken cancellationToken)
    {
        var command = new UpdateRoomTypeCommand(
            hotelId, id, request.Name, request.Description, request.PricePerNight, request.MaxAdults, request.MaxChildren);

        var result = await updateRoomType.HandleAsync(command, cancellationToken);

        return result.IsFailure ? HandleFailure(result.Error!) : NoContent();
    }

    /// <summary>Deletes a room type (soft delete).</summary>
    [HttpDelete("{id:int:min(1)}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Delete(int hotelId, int id, CancellationToken cancellationToken)
    {
        var result = await deleteRoomType.HandleAsync(new DeleteRoomTypeCommand(hotelId, id), cancellationToken);

        return result.IsFailure ? HandleFailure(result.Error!) : NoContent();
    }

    /// <summary>Adds an image to the room type's gallery.</summary>
    /// <response code="201">Added. "data" holds the image id.</response>
    /// <response code="400">Missing or too-long image URL.</response>
    /// <response code="404">No such hotel/room type, or it isn't yours.</response>
    [HttpPost("{id:int:min(1)}/images")]
    [ProducesResponseType<ApiResponse<IdResponse>>(StatusCodes.Status201Created)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddImage(int hotelId, int id, [FromBody] ImageRequest request, CancellationToken cancellationToken)
    {
        var result = await addRoomTypeImage.HandleAsync(new AddRoomTypeImageCommand(hotelId, id, request.ImageUrl), cancellationToken);

        return result.IsFailure
            ? HandleFailure(result.Error!)
            : StatusCode(StatusCodes.Status201Created, new ApiResponse<IdResponse>(new IdResponse(result.Value)));
    }

    /// <summary>Removes an image from the room type's gallery.</summary>
    /// <response code="204">Removed.</response>
    /// <response code="404">No such image on this room type, or it isn't yours.</response>
    [HttpDelete("{id:int:min(1)}/images/{imageId:int:min(1)}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveImage(int hotelId, int id, int imageId, CancellationToken cancellationToken)
    {
        var result = await removeRoomTypeImage.HandleAsync(new RemoveRoomTypeImageCommand(hotelId, id, imageId), cancellationToken);

        return result.IsFailure ? HandleFailure(result.Error!) : NoContent();
    }
}