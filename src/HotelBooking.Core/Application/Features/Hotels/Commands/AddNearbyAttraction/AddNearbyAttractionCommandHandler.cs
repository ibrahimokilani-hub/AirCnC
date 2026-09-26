using FluentValidation;
using HotelBooking.Core.Application.Abstractions;
using HotelBooking.Core.Application.Abstractions.Messaging;
using HotelBooking.Core.Application.Common.Validation;
using HotelBooking.Core.Domain.Common;
using HotelBooking.Core.Domain.Entities;

namespace HotelBooking.Core.Application.Features.Hotels.Commands.AddNearbyAttraction;

/// <summary>Creates a standalone catalog attraction. No hotel involved.</summary>
public sealed class AddNearbyAttractionCommandHandler(IAppDbContext context, IValidator<AddNearbyAttractionCommand> validator)
    : ICommandHandler<AddNearbyAttractionCommand, int>
{
    public async Task<Result<int>> HandleAsync(AddNearbyAttractionCommand command, CancellationToken cancellationToken)
    {
        var validation = await validator.ValidateAsync(command, cancellationToken);
        if (!validation.IsValid)
        {
            return Result<int>.Failure(validation.ToValidationError());
        }

        var attraction = NearbyAttraction.Create(
            command.Name, command.Category, command.Latitude, command.Longitude, command.AttractionImageUrl);

        context.NearbyAttractions.Add(attraction);
        await context.SaveChangesAsync(cancellationToken);

        return Result<int>.Success(attraction.Id);
    }
}
