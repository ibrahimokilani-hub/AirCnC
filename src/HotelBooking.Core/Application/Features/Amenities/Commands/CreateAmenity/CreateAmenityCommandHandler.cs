using FluentValidation;
using HotelBooking.Core.Application.Abstractions;
using HotelBooking.Core.Application.Abstractions.Messaging;
using HotelBooking.Core.Application.Common.Validation;
using HotelBooking.Core.Domain.Common;
using HotelBooking.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Core.Application.Features.Amenities.Commands.CreateAmenity;

public sealed class CreateAmenityCommandHandler(
    IAppDbContext context,
    IValidator<CreateAmenityCommand> validator)
    : ICommandHandler<CreateAmenityCommand, int>
{
    public async Task<Result<int>> HandleAsync(CreateAmenityCommand command, CancellationToken cancellationToken)
    {
        var validation = await validator.ValidateAsync(command, cancellationToken);
        if (!validation.IsValid)
        {
            return Result<int>.Failure(validation.ToValidationError());
        }

        var name = command.Name.Trim();

        if (await context.Amenities.AnyAsync(amenity => amenity.Name == name, cancellationToken))
        {
            return Result<int>.Failure(AmenityErrors.AlreadyExists(name));
        }

        var amenity = Amenity.Create(name);

        context.Amenities.Add(amenity);
        await context.SaveChangesAsync(cancellationToken);

        return Result<int>.Success(amenity.Id);
    }
}