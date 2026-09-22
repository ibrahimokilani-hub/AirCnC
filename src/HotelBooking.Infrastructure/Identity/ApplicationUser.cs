using Microsoft.AspNetCore.Identity;

namespace HotelBooking.Infrastructure.Identity;

public class ApplicationUser : IdentityUser<int>
{
    public const int NameMaxLength = 100;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
}