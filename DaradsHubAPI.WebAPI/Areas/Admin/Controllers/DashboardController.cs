using DaradsHubAPI.Core.Model;
using DaradsHubAPI.Core.Model.Request;
using DaradsHubAPI.Core.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace DaradsHubAPI.WebAPI.Areas.Admin.Controllers;

[Tags("Admin")]
public class DashboardController(IAdminService _adminService) : ApiBaseController
{
    [HttpGet("dashboard-metrics")]
    [ProducesResponseType(200, Type = typeof(ApiResponse<AdminDashboardMetricResponse>))]
    public async Task<IActionResult> GetDashboardMetrics()
    {
        var response = await _adminService.GetDashboardMetrics();
        return ResponseCode(response);
    }

    [HttpGet("daily-sales-overview")]
    [ProducesResponseType(200, Type = typeof(ApiResponse<DailySalesOverviewResponse>))]
    public async Task<IActionResult> DailySalesOverview()
    {
        var response = await _adminService.DailySalesOverview();
        return ResponseCode(response);
    }

    [HttpGet("top-performing-agents")]
    [ProducesResponseType(200, Type = typeof(ApiResponse<IEnumerable<TopPerformingAgentResponse>>))]
    public async Task<IActionResult> TopPerformingAgents()
    {
        var response = await _adminService.TopPerformingAgents();
        return ResponseCode(response);
    }

    [HttpGet("pending-customer-requests")]
    [ProducesResponseType(200, Type = typeof(ApiResponse<IEnumerable<LastFourCustomerRequest>>))]
    public async Task<IActionResult> PendingCustomerRequests()
    {
        var response = await _adminService.PendingCustomerRequests();
        return ResponseCode(response);
    }
}