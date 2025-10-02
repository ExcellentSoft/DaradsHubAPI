using System.ComponentModel.DataAnnotations;

namespace DaradsHubAPI.Domain.Entities;

public class ProductImages
{
    [Key]
    public int Id { get; set; }
    public long ProductId { get; set; }
    public string ImageUrl { get; set; } = default!;
    public bool Status { get; set; } = true;
}

public class DigitalProductImages
{
    [Key]
    public long Id { get; set; }
    public long ProductId { get; set; }
    public string ImageUrl { get; set; } = default!;
    public bool Status { get; set; } = true;
}


public class ProductRequestImages
{
    [Key]
    public long Id { get; set; }
    public long RequestId { get; set; }
    public string ImageUrl { get; set; } = default!;
}

public class SuspendedAgent
{
    [Key]
    public long Id { get; set; }
    public int AgentId { get; set; }
    public string Reason { get; set; } = default!;
    public string? OptionalNote { get; set; }
    public string? Duration { get; set; }

}

public class BlockedAgent
{
    [Key]
    public long Id { get; set; }
    public int AgentId { get; set; }
    public string Reason { get; set; } = default!;
}