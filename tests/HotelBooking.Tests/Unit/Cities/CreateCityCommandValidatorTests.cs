using FluentValidation.TestHelper;
using HotelBooking.Core.Application.Features.Cities.Commands.CreateCity;

namespace HotelBooking.Tests.Unit.Cities;

public sealed class CreateCityCommandValidatorTests
{
    private readonly CreateCityCommandValidator _validator = new();

    private static CreateCityCommand Valid() => new("Jenin", "Palestine", "P100");

    [Fact]
    public void Validate_ValidCommand_HasNoErrors() =>
        _validator.TestValidate(Valid()).ShouldNotHaveAnyValidationErrors();

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Validate_BlankName_HasNameError(string name) =>
        _validator.TestValidate(Valid() with { Name = name })
            .ShouldHaveValidationErrorFor(command => command.Name);

    [Fact]
    public void Validate_NameAtTheLimit_HasNoError() =>
        _validator.TestValidate(Valid() with { Name = new string('a', 70) })
            .ShouldNotHaveValidationErrorFor(command => command.Name);

    [Fact]
    public void Validate_NameOverTheLimit_HasNameError() =>
        _validator.TestValidate(Valid() with { Name = new string('a', 71) })
            .ShouldHaveValidationErrorFor(command => command.Name);

    [Fact]
    public void Validate_BlankCountry_HasCountryError() =>
        _validator.TestValidate(Valid() with { Country = "" })
            .ShouldHaveValidationErrorFor(command => command.Country);

    [Fact]
    public void Validate_CountryOverTheLimit_HasCountryError() =>
        _validator.TestValidate(Valid() with { Country = new string('a', 41) })
            .ShouldHaveValidationErrorFor(command => command.Country);

    [Fact]
    public void Validate_BlankPostOffice_HasPostOfficeError() =>
        _validator.TestValidate(Valid() with { PostOffice = "" })
            .ShouldHaveValidationErrorFor(command => command.PostOffice);

    [Fact]
    public void Validate_PostOfficeOverTheLimit_HasPostOfficeError() =>
        _validator.TestValidate(Valid() with { PostOffice = new string('1', 16) })
            .ShouldHaveValidationErrorFor(command => command.PostOffice);
}
