using HotelBooking.Contracts.Common;
using HotelBooking.Contracts.Hotels.Requests;
using HotelBooking.Contracts.Hotels.Responses;
using HotelBooking.Core.Application.Abstractions.Messaging;
using HotelBooking.Core.Application.Features.Hotels.Commands.AddNearbyAttraction;
using HotelBooking.Core.Application.Features.Hotels.Commands.RemoveNearbyAttraction;
using HotelBooking.Core.Application.Features.Hotels.Queries.GetNearbyAttractions;
using HotelBooking.Core.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.Api.Controllers.Admin;

[Route("api/v1/admin/attractions")]
[Tags("Admin · Attractions")]
[Authorize(Roles = nameof(UserRole.Admin))]
public sealed class AttractionsController(
    IQueryHandler<GetNearbyAttractionsQuery, PagedResult<NearbyAttractionResponse>> getAttractions,
    ICommandHandler<AddNearbyAttractionCommand, int> addAttraction,
    ICommandHandler<RemoveNearbyAttractionCommand> removeAttraction)
    : ApiController
{
    /// <summary>The catalog grid: paged, searchable by name or category, sortable.</summary>
    /// <remarks>sortBy: name (default), category. sortDirection: asc (default), desc.</remarks>
    /// <response code="200">One page of attractions, with paging metadata.</response>
    /// <response code="400">An invalid page or page size.</response>
    [HttpGet]
    [ProducesResponseType<PagedResponse<NearbyAttractionResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetList([FromQuery] FilteredRequest request, CancellationToken cancellationToken)
    {
        var query = new GetNearbyAttractionsQuery(request.Page, request.PageSize, request.Search, request.SortBy, request.SortDirection);

        var result = await getAttractions.HandleAsync(query, cancellationToken);

        return result.IsFailure ? HandleFailure(result.Error!) : OkPaged(result.Value);
    }

    /// <summary>Adds an attraction to the catalog.</summary>
    /// <response code="201">Created. "data" holds its id.</response>
    /// <response code="400">Invalid name, category or coordinates.</response>
    [HttpPost]
    [ProducesResponseType<ApiResponse<IdResponse>>(StatusCodes.Status201Created)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] NearbyAttractionRequest request, CancellationToken cancellationToken)
    {
        var command = new AddNearbyAttractionCommand(request.Name, request.Category, request.Latitude, request.Longitude, request.AttractionImageUrl);

        var result = await addAttraction.HandleAsync(command, cancellationToken);

        return result.IsFailure
            ? HandleFailure(result.Error!)
            : StatusCode(StatusCodes.Status201Created, new ApiResponse<IdResponse>(new IdResponse(result.Value)));
    }

    /// <summary>Deletes a catalog attraction.</summary>
    /// <response code="204">Deleted.</response>
    /// <response code="404">No such attraction.</response>
    [HttpDelete("{id:int:min(1)}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var result = await removeAttraction.HandleAsync(new RemoveNearbyAttractionCommand(id), cancellationToken);

        return result.IsFailure ? HandleFailure(result.Error!) : NoContent();
    }
}
