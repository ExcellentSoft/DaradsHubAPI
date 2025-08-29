using System.ComponentModel.DataAnnotations;
using static DaradsHubAPI.Domain.Enums.Enum;

namespace DaradsHubAPI.Domain.Entities;
#nullable disable
public class SubscriptionPlan
{
    [Key]
    public int Id { get; set; }
    [MaxLength(100)]
    public string Name { get; set; }
    public decimal Price { get; set; }
    public int? MaxLogTypes { get; set; }
}

public class VendorSubscriptionPlan
{
    [Key]
    public int Id { get; set; }
    public int UserId { get; set; }
    public int SubscriptionPlanId { get; set; }
    public bool IsActive { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}

public class MKTLogType
{
    [Key]
    public int Id { get; set; }
    public int SubPlanId { get; set; }
    [MaxLength(256)]
    public string Name { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal BulkPrice { get; set; }
    [MaxLength(256)]
    public string Image { get; set; }
    public DateTime TimeCreated { get; set; }
    public DateTime TimeUpdated { get; set; }
}

public class MKTLog
{
    [Key]
    public int Id { get; set; }
    public int LogTypeId { get; set; }
    [MaxLength(256)]
    public string Name { get; set; }
    [MaxLength(600)]
    public string Value { get; set; }
    [MaxLength(256)]
    public string ProfileLink { get; set; }
    [MaxLength(256)]
    public string Description { get; set; }
    [MaxLength(10)]
    public string Status { get; set; }
    public DateTime TimeCreated { get; set; }
    public DateTime TimeUpdated { get; set; }
}

public class MKTLogOrder
{
    [Key]
    public int Id { get; set; }
    public int VendorId { get; set; }
    public int CustomerId { get; set; }
    [MaxLength(250)]
    public string LogTypeName { get; set; }
    [MaxLength(500)]
    public string LogValue { get; set; }
    public decimal Amount { get; set; }
    public decimal ChargeAmount { get; set; }
    public TransactionStatusEnum Status { get; set; }
    public DateTime TimeCreated { get; set; }
    public DateTime TimeUpdated { get; set; }
}