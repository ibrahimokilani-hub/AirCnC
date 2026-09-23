using HotelBooking.Contracts.Amenities;
using HotelBooking.Contracts.Amenities.Requests;
using HotelBooking.Contracts.Amenities.Responses;
using HotelBooking.Contracts.Common;
using HotelBooking.Core.Application.Abstractions.Messaging;
using HotelBooking.Core.Application.Features.Amenities.Commands.CreateAmenity;
using HotelBooking.Core.Application.Features.Amenities.Commands.DeleteAmenity;
using HotelBooking.Core.Application.Features.Amenities.Queries.GetAmenities;
using HotelBooking.Core.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.Api.Controllers.Admin;

[Route("api/v1/admin/amenities")]
[Tags("Admin · Amenities")]
public sealed class AmenitiesController(
    ICommandHandler<CreateAmenityCommand, int> createAmenity,
    ICommandHandler<DeleteAmenityCommand> deleteAmenity,
    IQueryHandler<GetAmenitiesQuery, IReadOnlyList<AmenityResponse>> getAmenities)
    : ApiController
{
    [Authorize(Roles = nameof(UserRole.Admin))]
    [HttpGet]
    [ProducesResponseType<ApiResponse<IReadOnlyList<AmenityResponse>>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await getAmenities.HandleAsync(new GetAmenitiesQuery(), cancellationToken);

        return result.IsFailure ? HandleFailure(result.Error!) : OkData(result.Value);
    }

    /// <summary>Adds an amenity hotels can then pick</summary>
    [Authorize(Roles = nameof(UserRole.Admin))]
    [HttpPost]
    [ProducesResponseType<ApiResponse<IdResponse>>(StatusCodes.Status201Created)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] AmenityRequest request, CancellationToken cancellationToken)
    {
        var result = await createAmenity.HandleAsync(new CreateAmenityCommand(request.Name), cancellationToken);

        return result.IsFailure
            ? HandleFailure(result.Error!)
            : StatusCode(StatusCodes.Status201Created, new ApiResponse<IdResponse>(new IdResponse(result.Value)));
    }

    /// <summary>Deletes an amenity (soft delete)</summary>
    [Authorize(Roles = nameof(UserRole.Admin))]
    [HttpDelete("{id:int:min(1)}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var result = await deleteAmenity.HandleAsync(new DeleteAmenityCommand(id), cancellationToken);

        return result.IsFailure ? HandleFailure(result.Error!) : NoContent();
    }
}
