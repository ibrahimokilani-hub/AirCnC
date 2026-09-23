using FluentValidation;
using HotelBooking.Core.Application.Abstractions;
using HotelBooking.Core.Application.Abstractions.Messaging;
using HotelBooking.Core.Application.Common.Validation;
using HotelBooking.Core.Domain.Common;
using HotelBooking.Core.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Core.Application.Features.Hotels.Commands.UpdateHotel;

public sealed class UpdateHotelCommandHandler(
    IAppDbContext context,
    IHotelOwnership ownership,
    IValidator<UpdateHotelCommand> validator)
    : ICommandHandler<UpdateHotelCommand>
{
    public async Task<Result> HandleAsync(UpdateHotelCommand command, CancellationToken cancellationToken)
    {
        var validation = await validator.ValidateAsync(command, cancellationToken);
        if (!validation.IsValid)
        {
            return Result.Failure(validation.ToValidationError());
        }
        
        var access = await ownership.EnsureOwnerAsync(command.Id, cancellationToken);
        if (access.IsFailure)
        {
            return access;
        }

        var hotel = await context.Hotels.FirstOrDefaultAsync(hotel => hotel.Id == command.Id, cancellationToken);
        if (hotel is null)
        {
            return Result.Failure(HotelErrors.NotFound(command.Id));
        }

        var cityExists = await context.Cities.AnyAsync(city => city.Id == command.CityId, cancellationToken);
        if (!cityExists)
        {
            return Result.Failure(HotelErrors.UnknownCity(command.CityId));
        }

        hotel.Update(
            command.CityId,
            command.Name,
            command.Description,
            command.StarRating,
            Enum.Parse<HotelType>(command.HotelType, ignoreCase: true),
            command.Address,
            command.Latitude,
            command.Longitude);

        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
