using FluentValidation;
using HotelBooking.Core.Application.Abstractions;
using HotelBooking.Core.Application.Abstractions.Messaging;
using HotelBooking.Core.Application.Common.Validation;
using HotelBooking.Core.Domain.Common;
using HotelBooking.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Core.Application.Features.Cities.Commands.CreateCity;

public sealed class CreateCityCommandHandler(
    IAppDbContext context,
    IValidator<CreateCityCommand> validator)
    : ICommandHandler<CreateCityCommand, int>
{
    public async Task<Result<int>> HandleAsync(
        CreateCityCommand command,
        CancellationToken cancellationToken)
    {
        var validation = await validator.ValidateAsync(command, cancellationToken);
        if (!validation.IsValid)
        {
            return Result<int>.Failure(validation.ToValidationError());
        }

        var name = command.Name.Trim();
        var country = command.Country.Trim();

        var alreadyExists = await context.Cities
            .AnyAsync(city => city.Name == name && city.Country == country, cancellationToken);

        if (alreadyExists)
        {
            return Result<int>.Failure(CityErrors.AlreadyExists(name, country));
        }

        var city = City.Create(name, country, command.PostOffice);

        context.Cities.Add(city);
        await context.SaveChangesAsync(cancellationToken);

        return Result<int>.Success(city.Id);
    }
}