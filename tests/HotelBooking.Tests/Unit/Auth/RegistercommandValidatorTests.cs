using HotelBooking.Core.Application.Features.Auth.Register;

namespace HotelBooking.Tests.Unit.Auth;

public sealed class RegisterCommandValidatorTests
{
    private readonly RegisterCommandValidator _validator = new();

    private static RegisterCommand Valid() => new("sara@example.com", "Passw0rdOk", "Sara", "Haddad");

    /// <summary>The names of the properties that have at least one error.</summary>
    private string[] FailedProperties(RegisterCommand command) =>
        _validator.Validate(command).Errors.Select(error => error.PropertyName).Distinct().ToArray();

    [Fact]
    public void Validate_ValidCommand_HasNoErrors()
    {
        var result = _validator.Validate(Valid());

        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData("")]
    [InlineData("sara")]
    [InlineData("sara@")]
    public void Validate_BadEmail_HasEmailError(string email)
    {
        Assert.Contains(nameof(RegisterCommand.Email), FailedProperties(Valid() with { Email = email }));
    }

    [Fact]
    public void Validate_PasswordExactlyAtTheMinimum_HasNoError()
    {
        Assert.DoesNotContain(nameof(RegisterCommand.Password), FailedProperties(Valid() with { Password = "passwo" }));
    }

    [Fact]
    public void Validate_BlankFirstName_HasFirstNameError()
    {
        Assert.Contains(nameof(RegisterCommand.FirstName), FailedProperties(Valid() with { FirstName = " " }));
    }
}