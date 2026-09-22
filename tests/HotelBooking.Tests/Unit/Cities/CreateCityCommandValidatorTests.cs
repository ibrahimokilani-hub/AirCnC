using HotelBooking.Core.Application.Features.Cities.Commands.CreateCity;
using HotelBooking.Core.Domain.Entities;

namespace HotelBooking.Tests.Unit.Cities;

public sealed class CreateCityCommandValidatorTests
{
    private readonly CreateCityCommandValidator _validator = new();

    private static CreateCityCommand Valid() => new("Jenin", "Palestine", "P100");

    private string[] FailedProperties(CreateCityCommand command) =>
        _validator.Validate(command).Errors.Select(error => error.PropertyName).Distinct().ToArray();

    [Fact]
    public void Validate_ValidCommand_HasNoErrors()
    {
        Assert.True(_validator.Validate(Valid()).IsValid);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Validate_BlankName_HasNameError(string name)
    {
        Assert.Contains(nameof(CreateCityCommand.Name), FailedProperties(Valid() with { Name = name }));
    }

    [Fact]
    public void Validate_NameAtTheLimit_HasNoError()
    {
        var command = Valid() with { Name = new string('a', City.NameMaxLength) };

        Assert.DoesNotContain(nameof(CreateCityCommand.Name), FailedProperties(command));
    }

    [Fact]
    public void Validate_NameOverTheLimit_HasNameError()
    {
        var command = Valid() with { Name = new string('a', City.NameMaxLength + 1) };

        Assert.Contains(nameof(CreateCityCommand.Name), FailedProperties(command));
    }

    [Fact]
    public void Validate_BlankCountry_HasCountryError()
    {
        Assert.Contains(nameof(CreateCityCommand.Country), FailedProperties(Valid() with { Country = "" }));
    }

    [Fact]
    public void Validate_CountryOverTheLimit_HasCountryError()
    {
        var command = Valid() with { Country = new string('a', City.CountryMaxLength + 1) };

        Assert.Contains(nameof(CreateCityCommand.Country), FailedProperties(command));
    }

    [Fact]
    public void Validate_BlankPostOffice_HasPostOfficeError()
    {
        Assert.Contains(nameof(CreateCityCommand.PostOffice), FailedProperties(Valid() with { PostOffice = "" }));
    }

    [Fact]
    public void Validate_PostOfficeOverTheLimit_HasPostOfficeError()
    {
        var command = Valid() with { PostOffice = new string('1', City.PostOfficeMaxLength + 1) };

        Assert.Contains(nameof(CreateCityCommand.PostOffice), FailedProperties(command));
    }
}