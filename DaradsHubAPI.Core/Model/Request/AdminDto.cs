using DaradsHubAPI.Core.Model.Response;

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