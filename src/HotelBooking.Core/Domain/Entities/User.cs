using HotelBooking.Core.Domain.Common;
using HotelBooking.Core.Domain.Enums;

namespace HotelBooking.Core.Domain.Entities;

public sealed class User : Entity
{
    public const int EmailMaxLength = 256;
    public const int NameMaxLength = 100;
    public const int PasswordHashMaxLength = 200;

    User(string email, string firstName, string lastName, string passwordHash, UserRole role, DateTime createdAtUtc)
    {
        Email = email;
        FirstName = firstName;
        LastName = lastName;
        PasswordHash = passwordHash;
        Role = role;
        CreatedAtUtc = createdAtUtc;
    }

    public string Email { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string PasswordHash { get; set; }

    public UserRole Role { get; set; }

    public DateTime CreatedAtUtc { get; set; }

       public static User Create(string email, string firstName, string lastName, string passwordHash, DateTime createdAtUtc)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(email);
        ArgumentException.ThrowIfNullOrWhiteSpace(firstName);
        ArgumentException.ThrowIfNullOrWhiteSpace(lastName);
        ArgumentException.ThrowIfNullOrWhiteSpace(passwordHash);

        return new User(email.Trim().ToLowerInvariant(), firstName.Trim(), lastName.Trim(), passwordHash, UserRole.User, createdAtUtc);
    }

}
