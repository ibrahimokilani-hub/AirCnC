using HotelBooking.Core.Domain.Common;

namespace HotelBooking.Core.Domain.Entities;

public class City : AuditableEntity
{
    public const int NameMaxLength = 70;
    public const int CountryMaxLength = 40;
    public const int PostOfficeMaxLength = 15;
    
    private City(string name, string country, string postOffice)
    {
        Name = name;
        Country = country;
        PostOffice = postOffice;
    }
    
    public string Name { get; set; }
    public string Country { get; set; }
    public string PostOffice { get; set; }
    
    public static City Create(string name, string country, string postOffice)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(country);
        ArgumentException.ThrowIfNullOrWhiteSpace(postOffice);

        return new City(name.Trim(), country.Trim(), postOffice.Trim());
    }

    public void Update(string name, string country, string postOffice)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(country);
        ArgumentException.ThrowIfNullOrWhiteSpace(postOffice);

        Name = name.Trim();
        Country = country.Trim();
        PostOffice = postOffice.Trim();
    }
}