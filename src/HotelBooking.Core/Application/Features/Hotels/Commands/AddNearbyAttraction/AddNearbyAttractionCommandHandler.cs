using FluentValidation;
using HotelBooking.Core.Application.Abstractions;
using HotelBooking.Core.Application.Abstractions.Messaging;
using HotelBooking.Core.Application.Common.Validation;
using HotelBooking.Core.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Core.Application.Features.Hotels.Commands.AddNearbyAttraction;

public sealed class AddNearbyAttractionCommandHandler(
    IAppDbContext context,
    IHotelOwnership ownership,
    IValidator<AddNearbyAttractionCommand> validator)
    : ICommandHandler<AddNearbyAttractionCommand, int>
{
    public async Task<Result<int>> HandleAsync(AddNearbyAttractionCommand command, CancellationToken cancellationToken)
    {
        var access = await ownership.EnsureOwnerAsync(command.HotelId, cancellationToken);
        if (access.IsFailure)
        {
            return Result<int>.Failure(access.Error!);
        }
        
        var validation = await validator.ValidateAsync(command, cancellationToken);
        if (!validation.IsValid)
        {
            return Result<int>.Failure(validation.ToValidationError());
        }

        var hotel = await context.Hotels.FirstOrDefaultAsync(hotel => hotel.Id == command.HotelId, cancellationToken);
        if (hotel is null)
        {
            return Result<int>.Failure(HotelErrors.NotFound(command.HotelId));
        }

        var attraction = hotel.AddNearbyAttraction(command.Name, command.Category, command.Latitude, command.Longitude);
        await context.SaveChangesAsync(cancellationToken);

        return Result<int>.Success(attraction.Id);
    }
}