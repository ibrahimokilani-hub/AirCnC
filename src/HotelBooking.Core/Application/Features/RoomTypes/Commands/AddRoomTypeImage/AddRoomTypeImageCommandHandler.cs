using FluentValidation;
using HotelBooking.Core.Application.Abstractions;
using HotelBooking.Core.Application.Abstractions.Messaging;
using HotelBooking.Core.Application.Common.Validation;
using HotelBooking.Core.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Core.Application.Features.RoomTypes.Commands.AddRoomTypeImage;

public sealed class AddRoomTypeImageCommandHandler(
    IAppDbContext context,
    IHotelOwnership ownership,
    IValidator<AddRoomTypeImageCommand> validator)
    : ICommandHandler<AddRoomTypeImageCommand, int>
{
    public async Task<Result<int>> HandleAsync(AddRoomTypeImageCommand command, CancellationToken cancellationToken)
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

        // Scope to the hotel so an owner can't attach images to another hotel's room type.
        var roomType = await context.RoomTypes.FirstOrDefaultAsync(
            roomType => roomType.Id == command.RoomTypeId && roomType.HotelId == command.HotelId,
            cancellationToken);

        if (roomType is null)
        {
            return Result<int>.Failure(Error.NotFound(
                "RoomType.NotFound",
                $"Hotel '{command.HotelId}' has no room type with ID '{command.RoomTypeId}'."));
        }

        var image = roomType.AddImage(command.ImageUrl);
        await context.SaveChangesAsync(cancellationToken);

        return Result<int>.Success(image.Id);
    }
}
