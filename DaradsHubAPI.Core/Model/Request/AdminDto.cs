using DaradsHubAPI.Core.Model.Response;

namespace DaradsHubAPI.Core.Model.Request;

public class AdminDashboardMetricResponse
{
    public decimal? AgentRevenueBalance { get; set; }
    public decimal TotalWithdrawn { get; set; }
    public int ProductsCount { get; set; }
    public int TodayOrdersCount { get; set; }
}

public record DailySalesOverviewResponse
{
    public int AgentId { get; set; }
    public string AgentName { get; set; } = default!;
    public decimal Revenue { get; set; }
    public int Orders { get; set; }
}