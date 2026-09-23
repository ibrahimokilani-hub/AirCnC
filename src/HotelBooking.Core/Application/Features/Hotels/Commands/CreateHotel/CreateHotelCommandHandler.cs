using FluentValidation;
using HotelBooking.Core.Application.Abstractions;
using HotelBooking.Core.Application.Abstractions.Messaging;
using HotelBooking.Core.Application.Common.Validation;
using HotelBooking.Core.Domain.Common;
using HotelBooking.Core.Domain.Entities;
using HotelBooking.Core.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Core.Application.Features.Hotels.Commands.CreateHotel;

public sealed class CreateHotelCommandHandler(
    IAppDbContext context,
    ICurrentUser currentUser,
    IValidator<CreateHotelCommand> validator)
    : ICommandHandler<CreateHotelCommand, int>
{
    public async Task<Result<int>> HandleAsync(CreateHotelCommand command, CancellationToken cancellationToken)
    {
        var validation = await validator.ValidateAsync(command, cancellationToken);
        if (!validation.IsValid)
        {
            return Result<int>.Failure(validation.ToValidationError());
        }

        var cityExists = await context.Cities.AnyAsync(city => city.Id == command.CityId, cancellationToken);
        if (!cityExists)
        {
            return Result<int>.Failure(HotelErrors.UnknownCity(command.CityId));
        }

        var ownerId = currentUser.UserId
                      ?? throw new InvalidOperationException("The endpoint requires authentication.");

        var hotel = Hotel.Create(
            command.CityId,
            ownerId,
            command.Name,
            command.Description,
            command.StarRating,
            Enum.Parse<HotelType>(command.HotelType, ignoreCase: true),
            command.Address,
            command.Latitude,
            command.Longitude);

        context.Hotels.Add(hotel);
        await context.SaveChangesAsync(cancellationToken);

        return Result<int>.Success(hotel.Id);
    }
}