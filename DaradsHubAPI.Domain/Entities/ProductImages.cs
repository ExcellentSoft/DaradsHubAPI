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