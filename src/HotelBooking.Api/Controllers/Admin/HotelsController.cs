using HotelBooking.Contracts.Common;
using HotelBooking.Contracts.Hotels.Requests;
using HotelBooking.Contracts.Hotels.Responses;
using HotelBooking.Core.Application.Abstractions.Messaging;
using HotelBooking.Core.Application.Features.Hotels.Commands.CreateHotel;
using HotelBooking.Core.Application.Features.Hotels.Commands.DeleteHotel;
using HotelBooking.Core.Application.Features.Hotels.Commands.SetHotelAmenities;
using HotelBooking.Core.Application.Features.Hotels.Commands.UpdateHotel;
using HotelBooking.Core.Application.Features.Hotels.Queries.GetHotelById;
using HotelBooking.Core.Application.Features.Hotels.Queries.GetHotelsList;
using HotelBooking.Core.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.Api.Controllers.Admin;

[Route("api/v1/admin/hotels")]
[Tags("Admin · Hotels")]
public sealed class HotelsController(
    ICommandHandler<CreateHotelCommand, int> createHotel,
    ICommandHandler<UpdateHotelCommand> updateHotel,
    ICommandHandler<DeleteHotelCommand> deleteHotel,
    IQueryHandler<GetHotelByIdQuery, HotelResponse> getHotelById,
    ICommandHandler<SetHotelAmenitiesCommand> setHotelAmenities,
    IQueryHandler<GetHotelsListQuery, PagedResult<HotelListItem>> getHotelsList)
    : ApiController
{
    /// <summary>The admin grid: paged, searchable by name, owner or city, sortable.</summary>
    /// <remarks>sortBy: name (default), city, starRating, createdAt.</remarks>
    [HttpGet]
    [ProducesResponseType<PagedResponse<HotelListItem>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetList([FromQuery] FilteredRequest request, CancellationToken cancellationToken)
    {
        var query = new GetHotelsListQuery(request.Page, request.PageSize, request.Search, request.SortBy, request.SortDirection);

        var result = await getHotelsList.HandleAsync(query, cancellationToken);

        return result.IsFailure ? HandleFailure(result.Error!) : OkPaged(result.Value);
    }

    /// <summary>Creates a hotel in an existing city.</summary>
    /// <response code="201">Created. "data" holds its id.</response>
    /// <response code="400">Invalid fields, or a city that doesn't exist.</response>
    [Authorize(Roles = nameof(UserRole.Admin))]
    [HttpPost]
    [ProducesResponseType<ApiResponse<IdResponse>>(StatusCodes.Status201Created)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] HotelRequest request, CancellationToken cancellationToken)
    {
        var command = new CreateHotelCommand(
            request.CityId, request.Name, request.Description, request.StarRating,
            request.HotelType, request.Address, request.Latitude, request.Longitude);

        var result = await createHotel.HandleAsync(command, cancellationToken);

        return result.IsFailure
            ? HandleFailure(result.Error!)
            : CreatedAtAction(nameof(GetById), new { id = result.Value }, new ApiResponse<IdResponse>(new IdResponse(result.Value)));
    }

    /// <summary>Gets one hotel, for the edit form.</summary>
    [Authorize(Roles = nameof(UserRole.Admin))]
    [HttpGet("{id:int:min(1)}")]
    [ProducesResponseType<ApiResponse<HotelResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var result = await getHotelById.HandleAsync(new GetHotelByIdQuery(id), cancellationToken);

        return result.IsFailure ? HandleFailure(result.Error!) : OkData(result.Value);
    }

    /// <summary>Replaces every field of a hotel.</summary>
    [Authorize(Roles = nameof(UserRole.Admin))]
    [HttpPut("{id:int:min(1)}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, [FromBody] HotelRequest request, CancellationToken cancellationToken)
    {
        var command = new UpdateHotelCommand(
            id, request.CityId, request.Name, request.Description, request.StarRating,
            request.HotelType, request.Address, request.Latitude, request.Longitude);

        var result = await updateHotel.HandleAsync(command, cancellationToken);

        return result.IsFailure ? HandleFailure(result.Error!) : NoContent();
    }
    
    /// <summary>Replaces the hotel's amenities with exactly this list.</summary>
    /// <response code="204">Saved.</response>
    /// <response code="400">An amenity id that doesn't exist.</response>
    /// <response code="404">No such hotel.</response>
    [HttpPut("{id:int:min(1)}/amenities")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SetAmenities(
        int id,
        [FromBody] SetHotelAmenitiesRequest request,
        CancellationToken cancellationToken)
    {
        var result = await setHotelAmenities.HandleAsync(
            new SetHotelAmenitiesCommand(id, request.AmenityIds),
            cancellationToken);

        return result.IsFailure ? HandleFailure(result.Error!) : NoContent();
    }

    /// <summary>Deletes a hotel (soft delete).</summary>
    [Authorize(Roles = nameof(UserRole.Admin))]
    [HttpDelete("{id:int:min(1)}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var result = await deleteHotel.HandleAsync(new DeleteHotelCommand(id), cancellationToken);

        return result.IsFailure ? HandleFailure(result.Error!) : NoContent();
    }
}
