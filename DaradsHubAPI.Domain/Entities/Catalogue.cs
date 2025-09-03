using System.ComponentModel.DataAnnotations;

namespace DaradsHubAPI.Domain.Entities;

public partial class Catalogue
{
    [Key]
    public long Id { get; set; }
    [StringLength(250)]
    public string Name { get; set; } = default!;
    public DateTime DateCreated { get; set; }

}

public partial class CatalogueMapping
{
    [Key]
    public long Id { get; set; }
    public int AgentId { get; set; }
    public long CatalogueId { get; set; }
}


public partial class HubDigitalProduct
{
    [Key]
    public long Id { get; set; }
    [StringLength(250)]
    public string Title { get; set; } = default!;
    public int AgentId { get; set; }
    public long CatalogueId { get; set; }
    public decimal Price { get; set; }
    public decimal DiscountPrice { get; set; }
    public string? Description { get; set; }
    public DateTime DateCreated { get; set; }
    public DateTime DateUpdated { get; set; }
}
