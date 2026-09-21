using HotelBooking.Contracts.Cities.Requests;
using HotelBooking.Contracts.Cities.Responses;
using HotelBooking.Contracts.Common;
using HotelBooking.Core.Application.Abstractions.Messaging;
using HotelBooking.Core.Application.Features.Cities.Commands.CreateCity;
using HotelBooking.Core.Application.Features.Cities.Queries.GetCityById;
using HotelBooking.Core.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.Api.Controllers;

[Route("api/v1/cities")]
[Tags("Cities")]
public class CitiesController(
    ICommandHandler<CreateCityCommand, int> createCity,
    IQueryHandler<GetCityByIdQuery, CityResponse> getCityById)
    : ApiController
{
    /// <summary>Creates a city.</summary>
    /// <response code="201">The city was created.</response>
    /// <response code="400">The request failed validation.</response>
    /// <response code="409">A city with the same name and country already exists.</response>

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
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

        return CreatedAtAction(nameof(GetById), new {id = result.Value}, new ApiResponse<IdResponse>(new IdResponse(result.Value)));
    }

    /// <summary>Gets a single city by its id.</summary>
    /// <response code="200">The city.</response>
    /// <response code="404">No city has that id.</response>
    [HttpGet("{id:int:min(1)}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorResponse>( StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var query = new GetCityByIdQuery(id);
        var result = await getCityById.HandleAsync(query, cancellationToken);

        return result.IsFailure
            ? HandleFailure(result.Error!)
            : OkData(result);
    }
}