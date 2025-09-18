using DaradsHubAPI.Core.Model;
using DaradsHubAPI.Core.Model.Request;
using DaradsHubAPI.Core.Model.Response;
using DaradsHubAPI.Core.Services.Concrete;
using DaradsHubAPI.Core.Services.Interface;
using DaradsHubAPI.WebAPI.Extensions;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace DaradsHubAPI.WebAPI.Areas.Agent.Controllers;
[Tags("Agent")]
public class DashboardController(IAgentDashboardService _dashboardService) : ApiBaseController
{
    [HttpGet("metrics")]
    [ProducesResponseType(200, Type = typeof(ApiResponse<DashboardMetricResponse>))]
    public async Task<IActionResult> GetDashboardMetrics()
    {
        var agentId = int.Parse(User.Identity?.GetUserId() ?? "");
        var email = User.Identity?.GetUserEmail() ?? "";
        var response = await _dashboardService.GetDashboardMetrics(agentId, email);
        return ResponseCode(response);
    }

    [HttpGet("catalogue-insights")]
    [ProducesResponseType(200, Type = typeof(ApiResponse<CatalogueInsightResponse>))]
    public async Task<IActionResult> GetCatalogueInsight()
    {
        var agentId = int.Parse(User.Identity?.GetUserId() ?? "");
        var response = await _dashboardService.GetCatalogueInsight(agentId);
        return ResponseCode(response);
    }

    [HttpGet("latest-reviews")]
    [ProducesResponseType(200, Type = typeof(ApiResponse<IEnumerable<AgentReview>>))]
    public async Task<IActionResult> GetLatestReview()
    {
        var agentId = int.Parse(User.Identity?.GetUserId() ?? "");
        var response = await _dashboardService.GetLatestReview(agentId);
        return ResponseCode(response);
    }
}