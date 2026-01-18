namespace Bizonet.Api.Models.Entities;

public class City
{
    public int CityId { get; set; }      // cityID

    public int? StateId { get; set; }    // stateID nullable
    public string? CityName { get; set; } // cityName

    public byte So { get; set; } = 0;
    public byte Active { get; set; } = 1;

    // Navigation
    public State? State { get; set; }
}
