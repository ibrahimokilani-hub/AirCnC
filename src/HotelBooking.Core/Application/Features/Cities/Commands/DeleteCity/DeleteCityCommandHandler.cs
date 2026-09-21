using FluentValidation;
using HotelBooking.Core.Application.Abstractions;
using HotelBooking.Core.Application.Abstractions.Messaging;
using HotelBooking.Core.Application.Common.Validation;
using HotelBooking.Core.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Core.Application.Features.Cities.Commands.DeleteCity;

public sealed class DeleteCityCommandHandler(
    IAppDbContext context)
    : ICommandHandler<DeleteCityCommand>
{
    public async Task<Result> HandleAsync(
        DeleteCityCommand command,
        CancellationToken cancellationToken)
    {
        var city = await context.Cities.FirstOrDefaultAsync(c => c.Id == command.Id, cancellationToken);

        if (city is null)
        {
            return Result.Failure(CityErrors.NotFound(command.Id));
        }

        context.Cities.Remove(city);

        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}