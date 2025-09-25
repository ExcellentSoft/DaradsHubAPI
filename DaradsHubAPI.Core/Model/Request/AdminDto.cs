using DaradsHubAPI.Core.Model.Response;
using static DaradsHubAPI.Domain.Enums.Enum;

namespace DaradsHubAPI.Core.Model.Request;

public class AdminDashboardMetricResponse
{
    public decimal? AgentRevenueBalance { get; set; }
    public decimal TotalWithdrawn { get; set; }
    public int ProductsCount { get; set; }
    public OrderData? OrderData { get; set; }
}
public record OrderData
{
    public int TodayOrdersCount { get; set; }
    public decimal TodayOrderAmount { get; set; }
    public decimal PercentageChange { get; set; }
}

public record DailySalesOverviewResponse
{
    public DateTime Date { get; set; }
    public decimal TotalOrderAmount { get; set; }
    public IEnumerable<DailySalesOverview> DailySalesOverviews { get; set; } = default!;
}

public record DailySalesOverview
{
    public int AgentId { get; set; }
    public string AgentName { get; set; } = default!;
    public decimal Revenue { get; set; }
    public int Orders { get; set; }
}

public record TopPerformingAgentResponse
{
    public int AgentId { get; set; }
    public string AgentName { get; set; } = default!;
    public string? Photo { get; set; }
    public decimal Revenue { get; set; }
    public int OrdersCount { get; set; }
    public decimal TrendPercentage { get; set; }
}

public record LastFourCustomerRequest
{
    public long RequestId { get; set; }
    public int Quantity { get; set; }
    public string Reference { get; set; } = default!;
    public string ProductService { get; set; } = default!;
    public DateTime PreferredDate { get; set; }
    public string? Location { get; set; }
    public RequestStatus Status { get; set; }
    public DateTime DateCreated { get; set; }
    public RequestedUser? Customer { get; set; }

}

public record AgentVisibilityRequest
{
    public int AgentId { get; set; }
    public bool IsPublic { get; set; }
    public int ProductLimit { get; set; }
    public IEnumerable<int>? CategoriesIds { get; set; }
    public IEnumerable<long>? CataloguesIds { get; set; }
}

public record AgentStatusRequest
{
    public int AgentId { get; set; }
    public EntityStatusEnum EntityStatus { get; set; }

}

public record AgentsListRequest
{
    public int PageSize { get; set; } = 10;
    public int PageNumber { get; set; } = 1;
    public string? SearchText { get; set; }
    public int? Status { get; set; }
    public bool? IsPublic { get; set; }
    public string? ProductType { get; set; }
    public bool? CanSellPhysicalProducts { get; set; }
    public bool? CanSellDigitalProducts { get; set; }
}

public class AgentsListResponse
{
    public int AgentId { get; set; }
    public bool CanSellPhysicalProducts { get; internal set; }
    public bool CanSellDigitalProducts { get; internal set; }
    public string? FullName { get; set; }
    public int? Status { get; set; }
    public decimal? RevenueAmount { get; set; }
    public decimal WithdrawAmount { get; set; }
    public double MaxRating { get; set; }
    public int OrderCount { get; set; }
    public bool? IsPublic { get; set; }
    public string? Photo { get; set; }
    public string? LastActive { get; set; }
    public IEnumerable<string> ProductTypes
    {
        get
        {
            var types = new List<string>();
            if (CanSellDigitalProducts) types.Add("Digital");
            if (CanSellPhysicalProducts) types.Add("Physical");
            return types;
        }
    }
}

public class ProductTypeData
{
    public string Digital { get; set; } = default!;
    public string Physical { get; set; } = default!;

}