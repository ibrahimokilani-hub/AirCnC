using HotelBooking.Core.Application.Abstractions;
using HotelBooking.Core.Application.Abstractions.Messaging;
using HotelBooking.Core.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Core.Application.Features.Hotels.Commands.DeleteHotel;

public sealed class DeleteHotelCommandHandler(
    IAppDbContext context,
    IHotelOwnership ownership)
    : ICommandHandler<DeleteHotelCommand>
{
    public async Task<Result> HandleAsync(DeleteHotelCommand command, CancellationToken cancellationToken)
    {
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

        context.Hotels.Remove(hotel);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}