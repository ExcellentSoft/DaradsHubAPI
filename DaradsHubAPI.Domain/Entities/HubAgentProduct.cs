using System.ComponentModel.DataAnnotations;

namespace DaradsHubAPI.Domain.Entities;

public partial class HubProduct
{
    [Key]
    public long Id { get; set; }
    [MaxLength(250)]
    public string Name { get; set; } = default!;
}

public partial class HubAgentProduct
{
    [Key]
    public long Id { get; set; }
    [StringLength(250)]
    public string? Caption { get; set; }
    public int CategoryId { get; set; }
    public long SubCategoryId { get; set; }
    public string? Description { get; set; }
    public string? SKU { get; set; }
    public int AgentId { get; set; }
    public long ProductId { get; set; }
    public decimal Price { get; set; }
    public decimal DeliveryPrice { get; set; }
    public decimal DiscountPrice { get; set; }
    public bool IsFreeShipping { get; set; }
    public string? EstimateDeliveryTime { get; set; }
    public DateTime DateCreated { get; set; }
    public DateTime DateUpdated { get; set; }

}

public partial class HubAgentProfile
{
    [Key]
    public long Id { get; set; }
    public int UserId { get; set; }
    public string? Experience { get; set; }
}
