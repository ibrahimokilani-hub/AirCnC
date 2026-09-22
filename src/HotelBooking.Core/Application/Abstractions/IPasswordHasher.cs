namespace HotelBooking.Core.Application.Abstractions;

/// <summary>
/// Turns a password into something safe to store, and checks a password against it.
/// Core decides WHEN to hash; Infrastructure decides HOW (the algorithm and its cost).
/// </summary>
public interface IPasswordHasher
{
    string Hash(string password);

    bool Verify(string password, string passwordHash);
}
