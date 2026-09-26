using HotelBooking.Infrastructure.Security;

namespace HotelBooking.Tests.Unit.Security;

public sealed class Pbkdf2PasswordHasherTests
{
    private readonly Pbkdf2PasswordHasher _hasher = new();

    [Fact]
    public void Verify_TheSamePassword_Succeeds() =>
        Assert.True(_hasher.Verify("Passw0rdOk", _hasher.Hash("Passw0rdOk")));

    [Fact]
    public void Verify_AnotherPassword_Fails() =>
        Assert.False(_hasher.Verify("Passw0rdNo", _hasher.Hash("Passw0rdOk")));

    /// <summary>A fresh salt per hash: two users with one password must not share a hash.</summary>
    [Fact]
    public void Hash_TheSamePasswordTwice_GivesDifferentHashes() =>
        Assert.NotEqual(_hasher.Hash("Passw0rdOk"), _hasher.Hash("Passw0rdOk"));

    /// <summary>A stored hash that is damaged or from another scheme fails instead of throwing.</summary>
    [Theory]
    [InlineData("")]
    [InlineData("not-a-hash")]
    [InlineData("PBKDF2-SHA1.600000.c2FsdA==.aGFzaA==")]
    [InlineData("PBKDF2-SHA256.not-a-number.c2FsdA==.aGFzaA==")]
    [InlineData("PBKDF2-SHA256.600000.not-base64!.aGFzaA==")]
    public void Verify_AMalformedHash_ReturnsFalse(string passwordHash) =>
        Assert.False(_hasher.Verify("Passw0rdOk", passwordHash));
}