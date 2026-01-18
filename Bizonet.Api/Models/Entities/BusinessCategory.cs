namespace Bizonet.Api.Models.Entities;

public class BusinessCategory
{
    public int BusinessCategoryId { get; set; }   // businessCategoryID
    public string? CategoryName { get; set; }     // categoryName

    public int So { get; set; } = 0;              // mediumint -> int
    public byte Active { get; set; } = 1;
}
