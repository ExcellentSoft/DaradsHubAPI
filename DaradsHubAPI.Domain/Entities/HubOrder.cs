using System.ComponentModel.DataAnnotations;
using static DaradsHubAPI.Domain.Enums.Enum;

namespace DaradsHubAPI.Domain.Entities;
public partial class HubOrder
{
    [Key]
    public long Id { get; set; }
    public int AgentId { get; set; }
    [MaxLength(50)]
    public string UserEmail { get; set; } = default!;
    [MaxLength(30)]
    public string Code { get; set; } = default!;
    public decimal TotalCost { get; set; }
    [MaxLength(30)]
    public string ProductType { get; set; } = default!;
    public OrderStatus Status { get; set; }
    public DateTime OrderDate { get; set; }
    public int ShippingAddressId { get; set; }
}

public partial class HubOrderItem
{
    [Key]
    public long Id { get; set; }
    [MaxLength(30)]
    public string OrderCode { get; set; } = default!;
    public long ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public DateTime CreatedDate { get; set; }
}

public partial class HubOrderTracking
{
    [Key]
    public long Id { get; set; }
    [MaxLength(30)]
    public string OrderCode { get; set; } = default!;
    public OrderStatus Status { get; set; }
    [MaxLength(200)]
    public string? Description { get; set; }
    public DateTime DateCreated { get; set; }
}