namespace Bizonet.Api.Models.Entities;

public class Country
{
    public int CountryId { get; set; }          // countryID

    public string? CountryCode { get; set; }    // countryCode (3 chars)
    public string? CountryName { get; set; }    // countryName
    public int? PhoneCode { get; set; }         // phonecode

    public byte So { get; set; } = 0;           // sort order (so)
    public byte Active { get; set; } = 1;       // tinyint (0/1)

    // Navigation
    public ICollection<State> States { get; set; } = new List<State>();
}
