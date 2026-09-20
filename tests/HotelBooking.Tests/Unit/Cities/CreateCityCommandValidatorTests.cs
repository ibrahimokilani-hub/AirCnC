using FluentValidation.TestHelper;
using HotelBooking.Core.Application.Features.Cities.Commands.CreateCity;

namespace HotelBooking.Tests.Unit.Cities;

public class CreateCityCommandValidatorTests
{
    private readonly  CreateCityCommandValidator _validator = new();
    private static CreateCityCommand Valid() => new("Jenin", "Palestine", "11111");

    [Fact]
    public void HasNoErrors() => _validator.TestValidate(Valid()).ShouldNotHaveAnyValidationErrors();

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("  ")]
    public void HasNameErrors(string name) => _validator.TestValidate(Valid() with { Name = name })
        .ShouldHaveValidationErrorFor(command => command.Name);
    
    [Fact]
    public void Validate_NameTooLong_HasNameError() =>
        _validator.TestValidate(Valid() with { Name = new string('a', 71) })
            .ShouldHaveValidationErrorFor(command => command.Name);

    [Fact]
    public void Validate_BlankPostOffice_HasPostOfficeError() =>
        _validator.TestValidate(Valid() with { PostOffice = "" })
            .ShouldHaveValidationErrorFor(command => command.PostOffice);
}