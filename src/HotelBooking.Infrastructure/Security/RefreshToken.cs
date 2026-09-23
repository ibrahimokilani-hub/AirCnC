namespace HotelBooking.Infrastructure.Security;

public sealed class RefreshToken
{
    public int Id { get; private set; }

    public int UserId { get; private set; }

    public string TokenHash { get; private set; } = string.Empty;

    public DateTime CreatedAtUtc { get; private set; }

    public DateTime ExpiresAtUtc { get; private set; }

    public DateTime? RevokedAtUtc { get; private set; }

    public static RefreshToken Create(int userId, string tokenHash, DateTime now, DateTime expires) =>
        new()
        {
            UserId = userId,
            TokenHash = tokenHash,
            CreatedAtUtc = now,
            ExpiresAtUtc = expires
        };

    public bool IsActive(DateTime now) => RevokedAtUtc is null && now < ExpiresAtUtc;

    public void Revoke(DateTime now) => RevokedAtUtc = now;
}
