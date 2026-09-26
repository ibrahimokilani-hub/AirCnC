using HotelBooking.Core.Domain.Entities;
using HotelBooking.Core.Domain.Enums;

namespace HotelBooking.Tests.Unit.Domain;

public sealed class UserTests
{
    private static readonly DateTime NowUtc = new(2026, 6, 10, 9, 0, 0, DateTimeKind.Utc);

    private static User Create(
        string email = "sara@example.com",
        string firstName = "Sara",
        string lastName = "Haddad",
        string phone = "+970599123456",
        string passwordHash = "hash") =>
        User.Create(email, firstName, lastName, phone, passwordHash, NowUtc);

    /// <summary>Email is the login key, so it is stored in one canonical form.</summary>
    [Fact]
    public void Create_LowercasesAndTrimsTheEmail() =>
        Assert.Equal("sara@example.com", Create(email: "  Sara@Example.COM  ").Email);

    [Fact]
    public void Create_TrimsTheNamesAndPhone()
    {
        var user = Create(firstName: " Sara ", lastName: " Haddad ", phone: " +970599123456 ");

        Assert.Equal("Sara", user.FirstName);
        Assert.Equal("Haddad", user.LastName);
        Assert.Equal("+970599123456", user.Phone);
    }

    /// <summary>Nobody registers as an admin; the role is granted later.</summary>
    [Fact]
    public void Create_StartsAsAPlainUser() =>
        Assert.Equal(UserRole.User, Create().Role);

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Create_BlankEmail_Throws(string email) =>
        Assert.Throws<ArgumentException>(() => Create(email: email));

    [Fact]
    public void Create_WithoutAPasswordHash_Throws() =>
        Assert.Throws<ArgumentException>(() => Create(passwordHash: ""));
}