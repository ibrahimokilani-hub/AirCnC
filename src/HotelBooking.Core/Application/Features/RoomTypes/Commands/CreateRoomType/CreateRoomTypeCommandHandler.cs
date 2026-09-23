using FluentValidation;
using HotelBooking.Core.Application.Abstractions;
using HotelBooking.Core.Application.Abstractions.Messaging;
using HotelBooking.Core.Application.Common.Validation;
using HotelBooking.Core.Application.Features.Hotels;
using HotelBooking.Core.Domain.Common;
using HotelBooking.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Core.Application.Features.RoomTypes.Commands.CreateRoomType;

public sealed class CreateRoomTypeCommandHandler(
    IAppDbContext context,
    IHotelOwnership ownership,
    IValidator<CreateRoomTypeCommand> validator)
    : ICommandHandler<CreateRoomTypeCommand, int>
{
    public async Task<Result<int>> HandleAsync(CreateRoomTypeCommand command, CancellationToken cancellationToken)
    {
        var validation = await validator.ValidateAsync(command, cancellationToken);
        if (!validation.IsValid)
        {
            return Result<int>.Failure(validation.ToValidationError());
        }
        
        var access = await ownership.EnsureOwnerAsync(command.HotelId, cancellationToken);
        if (access.IsFailure)
        {
            return Result<int>.Failure(access.Error!);
        }

        var roomType = RoomType.Create(
            command.HotelId,
            command.Name,
            command.Description,
            command.PricePerNight,
            command.MaxAdults,
            command.MaxChildren);

        context.RoomTypes.Add(roomType);
        await context.SaveChangesAsync(cancellationToken);

        return Result<int>.Success(roomType.Id);
    }
}