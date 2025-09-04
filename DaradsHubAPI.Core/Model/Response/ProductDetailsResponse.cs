namespace DaradsHubAPI.Core.Model.Response
{
    public class ProductDetailsResponse
    {
        public long ProductId { get; set; }
        public string? Name { get; set; }
        public string? Caption { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }
        public int MaxRating { get; set; }
        public int ReviewCount { get; set; }
    }
    public class DigitalProductDetailsResponse
    {
        public long ProductId { get; set; }
        public string? Name { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }
        public int MaxRating { get; set; }
        public int ReviewCount { get; set; }
    }
    public class ProductDetailResponse
    {
        public long ProductId { get; set; }
        public string? Name { get; set; }
        public string? AgentName { get; set; }
        public string? Caption { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public List<string> ImageUrl { get; set; } = default!;
        public int MaxRating { get; set; }
        public int ReviewCount { get; set; }
    }

    public class DigitalProductDetailResponse
    {
        public long ProductId { get; set; }
        public string? Name { get; set; }
        public string? AgentName { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public List<string> ImageUrl { get; set; } = default!;
        public int MaxRating { get; set; }
        public int ReviewCount { get; set; }
    }

    public class LandingProductResponse
    {
        public long Id { get; set; }
        public long AgentId { get; set; }
        public string? Description { get; set; }
        public string? Caption { get; set; }
        public string? ImageUrl { get; set; }
    }
    public class LandingPageDigitalProductResponse
    {
        public long Id { get; set; }
        public long AgentId { get; set; }
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
    }

    public class AgentProductProfileResponse
    {
        public int AgentId { get; set; }
        public IEnumerable<string> SellingProducts { get; set; } = default!;
        public string? BusinessName { get; set; }
        public string? FullName { get; set; }
        public string? Experience { get; set; }
        public string? Address { get; set; }
        public string? State { get; set; }
        public string? City { get; set; }
        public int MaxRating { get; set; }
        public int ReviewCount { get; set; }
        public bool IsVerify { get; set; }
        public bool IsOnline { get; set; }
        public string? Photo { get; set; }
    }
    public class AgentsProfileResponse
    {
        public int AgentId { get; set; }
        public IEnumerable<string> SellingProducts { get; set; } = default!;
        public string? BusinessName { get; set; }
        public string? FullName { get; set; }
        public string? Experience { get; set; }
        public AgentsAddress? AgentsAddress { get; set; }
        public double MaxRating { get; set; }
        public int ReviewCount { get; set; }
        public bool IsVerify { get; set; }
        public bool IsOnline { get; set; }
        public string? Photo { get; set; }
    }

    public class AgentsAddress
    {
        public string? Address { get; set; }
        public string? State { get; set; }
        public string? City { get; set; }
    }
}
