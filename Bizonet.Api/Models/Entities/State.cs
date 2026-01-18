namespace Bizonet.Api.Models.Entities;

public class State
{
    public int StateId { get; set; }       // stateID

    public int CountryId { get; set; } = 1;  // countryID default 1
    public string? StateName { get; set; }   // stateName

    public byte So { get; set; } = 0;
    public byte Active { get; set; } = 1;

    // Navigation
    public Country Country { get; set; } = null!;
    public ICollection<City> Cities { get; set; } = new List<City>();
}
