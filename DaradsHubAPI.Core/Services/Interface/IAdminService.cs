using DaradsHubAPI.Core.Model;
using DaradsHubAPI.Core.Model.Request;

namespace DaradsHubAPI.Core.Services.Interface;
public interface IAdminService
{
    Task<ApiResponse<DailySalesOverviewResponse>> DailySalesOverview();
    Task<ApiResponse<AdminDashboardMetricResponse>> GetDashboardMetrics();
    Task<ApiResponse<IEnumerable<LastFourCustomerRequest>>> PendingCustomerRequests();
    Task<ApiResponse> SendBulkMessages(SendBulkMessageRequest request);
    Task<ApiResponse<IEnumerable<TopPerformingAgentResponse>>> TopPerformingAgents();
}
