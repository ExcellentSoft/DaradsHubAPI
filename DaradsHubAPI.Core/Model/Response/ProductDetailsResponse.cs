using System.ComponentModel.DataAnnotations;

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
        public long CatalogueId { get; internal set; }
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
        public IEnumerable<string> ImageUrls { get; set; } = default!;
    }
    public class SimilarProductResponse
    {
        public long Id { get; set; }
        public long AgentId { get; set; }
        public string? Description { get; set; }
        public string? Caption { get; set; }
        public string? Name { get; set; }
        public decimal Price { get; set; }
        public IEnumerable<string> ImageUrls { get; set; } = default!;
    }
    public class LandingPageDigitalProductResponse
    {
        public long Id { get; set; }
        public long AgentId { get; set; }
        public string? Description { get; set; }
        public IEnumerable<string> ImageUrls { get; set; } = default!;
        public string? Title { get; set; }
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
        public List<string> AnotherSellingProducts { get; internal set; }
        public string PhoneNumber { get; internal set; }
    }
    public class AgentsProfileResponse
    {
        public int AgentId { get; set; }
        public IEnumerable<SellingProduct> SellingProducts { get; set; } = default!;
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

    public class SellingProduct
    {
        public string? Name { get; set; }
        public string? Image { get; set; }
    }

    public class HubFAQResponse
    {
        public string Question { get; set; } = default!;
        public string Answer { get; set; } = default!;
    }
    public class ProductMetricResponse
    {
        public int TotalProduct { get; set; }
        public int TotalDigitalProductCount { get; set; }
        public int TotalPhysicalProduct { get; set; }
        public int TotalActiveChats { get; set; }
        public int TotalPendingReplies { get; set; }
    }

    public class DashboardMetricResponse
    {
        public decimal? Earning { get; set; }
        public int TotalProductCount { get; set; }
        public int TotalRequestCount { get; set; }
        public OrderDataResponse? OrderData { get; set; }
    }

    public class OrderDataResponse
    {
        public int TotalOrderCount { get; set; }
        public int TotalPendingCount { get; set; }
        public int TotalFulfillCount { get; set; }
    }

    public class ProductOrderMetricResponse
    {
        public int TotalOrderCount { get; set; }
        public int RefundedOrderCount { get; set; }
        public int CanceledOrderCount { get; set; }
        public int CompletedOrderCount { get; set; }
        public int PendingOrderCount { get; internal set; }
        public int ProcessingOrderCount { get; internal set; }
    }
    public class CustomerRequestMetricResponse
    {
        public int TotalCount { get; set; }
        public int RejectCount { get; set; }
        public int ApproveCount { get; set; }
        public int PendingCount { get; internal set; }

    }

    public class CustomerMetricsResponse
    {
        public int TotalCustomerCount { get; set; }
        public int TotalActiveChatCount { get; set; }
        public int TotalInActiveCount { get; set; }
        public NewCustomerModel? NewCustomerModel { get; set; }

    }
    public class NewCustomerModel
    {
        public int TotalNewCustomerCount { get; set; }
        public string Month { get; set; } = default!;
    }
}
