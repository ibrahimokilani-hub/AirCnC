using FluentValidation;
using HotelBooking.Core.Application.Abstractions;
using HotelBooking.Core.Application.Abstractions.Messaging;
using HotelBooking.Core.Application.Common.Validation;
using HotelBooking.Core.Domain.Common;
using HotelBooking.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Core.Application.Features.Cities.Commands.UpdateCity;

public sealed class UpdateCityCommandHandler(
    IAppDbContext context,
    IValidator<UpdateCityCommand> validator)
    : ICommandHandler<UpdateCityCommand>
{
    public async Task<Result> HandleAsync(
        UpdateCityCommand command,
        CancellationToken cancellationToken)
    {
        var validation = await validator.ValidateAsync(command, cancellationToken);
        if (!validation.IsValid)
        {
            return Result.Failure(validation.ToValidationError());
        }

       var city = context.Cities.FirstOrDefault(c => c.Id == command.Id);
        
        if (city is null)
        {
            return Result.Failure(CityErrors.NotFound(command.Id));
        }
        
        var name = command.Name.Trim();
        var country = command.Country.Trim();

        // Another city already has the new name and country.
        var takenByAnother = await context.Cities.AnyAsync(
            city => city.Id != command.Id && city.Name == name && city.Country == country,
            cancellationToken);

        if (takenByAnother)
        {
            return Result.Failure(CityErrors.AlreadyExists(name, country));
        }

        city.Update(name, country, command.PostOffice);
        
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}