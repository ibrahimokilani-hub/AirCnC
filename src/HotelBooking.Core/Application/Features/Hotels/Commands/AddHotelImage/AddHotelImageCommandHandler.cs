using FluentValidation;
using HotelBooking.Core.Application.Abstractions;
using HotelBooking.Core.Application.Abstractions.Messaging;
using HotelBooking.Core.Application.Common.Validation;
using HotelBooking.Core.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Core.Application.Features.Hotels.Commands.AddHotelImage;

public sealed class AddHotelImageCommandHandler(
    IAppDbContext context,
    IHotelOwnership ownership,
    IValidator<AddHotelImageCommand> validator)
    : ICommandHandler<AddHotelImageCommand, int>
{
    public async Task<Result<int>> HandleAsync(AddHotelImageCommand command, CancellationToken cancellationToken)
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

        var hotel = await context.Hotels.FirstOrDefaultAsync(hotel => hotel.Id == command.HotelId, cancellationToken);
        if (hotel is null)
        {
            return Result<int>.Failure(HotelErrors.NotFound(command.HotelId));
        }

        var image = hotel.AddImage(command.ImageUrl);
        await context.SaveChangesAsync(cancellationToken);

        return Result<int>.Success(image.Id);
    }
}
