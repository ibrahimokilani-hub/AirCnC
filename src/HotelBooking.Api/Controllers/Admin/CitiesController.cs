using HotelBooking.Contracts.Cities.Requests;
using HotelBooking.Contracts.Cities.Responses;
using HotelBooking.Contracts.Common;
using HotelBooking.Core.Application.Abstractions.Messaging;
using HotelBooking.Core.Application.Features.Cities.Commands.CreateCity;
using HotelBooking.Core.Application.Features.Cities.Commands.DeleteCity;
using HotelBooking.Core.Application.Features.Cities.Commands.UpdateCity;
using HotelBooking.Core.Application.Features.Cities.Queries.GetCitiesGrid;
using HotelBooking.Core.Application.Features.Cities.Queries.GetCityById;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.Api.Controllers.Admin;

[Route("api/v1/admin/cities")]
[Authorize]
[Tags("Admin · Cities")]
public sealed class CitiesController(
    IQueryHandler<GetCitiesGridQuery, PagedResult<CityGridItem>> getCitiesGrid,
    ICommandHandler<CreateCityCommand, int> createCity,
    ICommandHandler<UpdateCityCommand> updateCity,
    ICommandHandler<DeleteCityCommand> deleteCity,
    IQueryHandler<GetCityByIdQuery, CityResponse> getCityById)
    : ApiController
{
    /// <summary>Gets a paginated list of cities.</summary>
    /// <response code="200">The list of cities inside data, and the metadata inside meta.</response>
    /// <response code="404">No city has that id.</response>
    [HttpGet]
    [ProducesResponseType<ApiResponse<PagedResponse<CityResponse>>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetGrid([FromQuery] FilteredRequest request, CancellationToken cancellationToken)
    {
        var query = new GetCitiesGridQuery(
            request.Page, 
            request.PageSize, 
            request.Search, 
            request.SortBy, 
            request.SortDirection);

        var result = await getCitiesGrid.HandleAsync(query, cancellationToken);

        return result.IsFailure
            ? HandleFailure(result.Error!)
            : OkPaged(result.Value);
    }
    
    /// <summary>Creates a city.</summary>
    /// <response code="201">The city was created. "data" holds its id.</response>
    /// <response code="400">The request failed validation.</response>
    /// <response code="409">A city with the same name and country already exists.</response>
    [HttpPost]
    [ProducesResponseType<ApiResponse<IdResponse>>(StatusCodes.Status201Created)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create(
        [FromBody] CreateCityRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateCityCommand(request.Name, request.Country, request.PostOffice);

        var result = await createCity.HandleAsync(command, cancellationToken);

        if (result.IsFailure)
        {
            return HandleFailure(result.Error!);
        }

        return CreatedAtAction(
            nameof(GetById),
            new { id = result.Value },
            new ApiResponse<IdResponse>(new IdResponse(result.Value)));
    }

    /// <summary>Gets a single city by its id.</summary>
    /// <response code="200">The city.</response>
    /// <response code="404">No city has that id.</response>
    [HttpGet("{id:int:min(1)}")]
    [ProducesResponseType<ApiResponse<CityResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var result = await getCityById.HandleAsync(new GetCityByIdQuery(id), cancellationToken);

        return result.IsFailure
            ? HandleFailure(result.Error!)
            : OkData(result.Value);
    }

    /// <summary>Replaces a city's name, country and post office.</summary>
    /// <response code="204">The city was updated.</response>
    /// <response code="400">The request failed validation.</response>
    /// <response code="404">No city has that id.</response>
    /// <response code="409">Another city already has that name and country.</response>
    [HttpPut("{id:int:min(1)}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(
        int id,
        [FromBody] UpdateCityRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateCityCommand(id, request.Name, request.Country, request.PostOffice);

        var result = await updateCity.HandleAsync(command, cancellationToken);

        return result.IsFailure ? HandleFailure(result.Error!) : NoContent();
    }

    /// <summary>Deletes a city. The row is kept and hidden (soft delete).</summary>
    /// <response code="204">The city was deleted.</response>
    /// <response code="404">No city has that id.</response>
    [HttpDelete("{id:int:min(1)}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var result = await deleteCity.HandleAsync(new DeleteCityCommand(id), cancellationToken);

        return result.IsFailure ? HandleFailure(result.Error!) : NoContent();
    }
}
