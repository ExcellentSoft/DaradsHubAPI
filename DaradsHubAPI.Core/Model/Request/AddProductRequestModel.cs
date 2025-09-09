using Microsoft.AspNetCore.Http;
using static DaradsHubAPI.Domain.Enums.Enum;

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

public class AddAgentHubProductRequest
{
    public string? Caption { get; set; }
    public int ProductId { get; set; }
    public int CategoryId { get; set; }
    public long SubCategoryId { get; set; }
    public string? Description { get; set; }
    public string? SKU { get; set; }
    public decimal Price { get; set; }
    public decimal DeliveryPrice { get; set; }
    public decimal DiscountPrice { get; set; }
    public bool IsFreeShipping { get; set; }
    public string? EstimateDeliveryTime { get; set; }
    public IEnumerable<IFormFile> Images { get; set; } = default!;
}

public class CreateHubProductRequest
{
    public int AgentId { get; set; }
    public int Quantity { get; set; }
    public string CustomerNeed { get; set; } = default!;
    public string? Description { get; set; }
    public DateTime PreferredDate { get; set; }
    public decimal Budget { get; set; }
    public string Location { get; set; } = default!;
    public bool IsUrgent { get; set; }
    public ProductRequestTypeEnum ProductRequestTypeEnum { get; set; }
    public IEnumerable<IFormFile>? ReferenceFiles { get; set; }
    public int CategoryId { get; set; }
}

public class UpdateAgentHubProductRequest : AddAgentHubProductRequest
{
    public long Id { get; set; }
}

public class AddDigitalHubProductRequest
{
    public string Title { get; set; } = default!;
    public long CatalogueId { get; set; }
    public decimal Price { get; set; }
    public decimal DiscountPrice { get; set; }
    public string? Description { get; set; }
    public IEnumerable<IFormFile> Images { get; set; } = default!;
}

public class UpdateDigitalHubProductRequest : AddDigitalHubProductRequest
{
    public long Id { get; set; }
}


