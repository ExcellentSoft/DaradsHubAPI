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