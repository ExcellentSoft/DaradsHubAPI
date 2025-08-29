using Microsoft.AspNetCore.Http;

namespace DaradsHubAPI.Core.Model.Request;

public class AddProductRequestModel
{
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int CategoryId { get; set; }
    public int Qty { get; set; }
    public List<string>? Colors { get; set; }
    public List<string>? Sizes { get; set; }
    public int Stock { get; set; }
    public int VendorId { get; set; }
}

public class UpdateProductRequestModel : AddProductRequestModel
{
    public long Id { get; set; }
}

public record AddProductImageRequest
{
    public long ProductId { get; set; }
    public IFormFile Image { get; set; } = default!;
}