using DaradsHubAPI.Core.Model;
using DaradsHubAPI.Core.Model.Request;
using DaradsHubAPI.Core.Model.Response;
using DaradsHubAPI.Core.Services.Concrete;
using DaradsHubAPI.Core.Services.Interface;
using DaradsHubAPI.Domain.Entities;
using DaradsHubAPI.WebAPI.Extensions;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace DaradsHubAPI.WebAPI.Areas.Admin.Controllers;

[Tags("Admin")]
public class ManageAgentController(IManageAgentService _agentService, IProductService _productService) : ApiBaseController
{
    [HttpGet("view-agent-profile")]
    [ProducesResponseType(typeof(ApiResponse<ShortAgentProfileResponse>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetAgentProfile(int agentId)
    {
        var response = await _agentService.GetAgentProfile(agentId);
        return ResponseCode(response);
    }

    [HttpPatch("update-agent-visibility")]
    [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> UpdateAgentVisibility([FromBody] AgentVisibilityRequest request)
    {
        var response = await _agentService.UpdateAgentVisibility(request);
        return ResponseCode(response);
    }

    [HttpPatch("update-agent-status")]
    [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> UpdateAgentStatus([FromBody] AgentStatusRequest request)
    {
        var response = await _agentService.UpdateAgentStatus(request);
        return ResponseCode(response);
    }

    [HttpGet("view-agents")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<AgentsListResponse>>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetAgents([FromQuery] AgentsListRequest request)
    {
        var response = await _agentService.GetAgents(request);
        return ResponseCode(response);

    }

    [HttpPost("create-agent-account")]
    [ProducesResponseType(typeof(ApiResponse<string>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> AddAgent([FromForm] AddAgentRequest request)
    {
        var response = await _agentService.CreateAgent(request);
        return ResponseCode(response);
    }

    [HttpPatch("toggle-agent-visibility")]
    [ProducesResponseType(typeof(ApiResponse<bool>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> ToggleVisibility([FromQuery] int agentId, [FromQuery] bool isPublic)
    {
        var response = await _agentService.ToggleVisibility(agentId, isPublic);
        if (!response.Status.GetValueOrDefault())
        {
            return BadRequest(response);
        }
        return Ok(response);
    }

    [HttpGet("agent-dashboard-metrics")]
    [ProducesResponseType(200, Type = typeof(ApiResponse<AgentDashboardMetricResponse>))]
    public async Task<IActionResult> GetDashboardMetrics([FromQuery] int agentId)
    {
        var response = await _agentService.GetAgentDashboardMetrics(agentId);
        return ResponseCode(response);
    }

    [HttpGet("agent-products")]
    [ProducesResponseType(200, Type = typeof(ApiResponse<IEnumerable<AgentProductsResponse>>))]
    public async Task<IActionResult> GetProducts([FromQuery] AgentProductsRequest request, [FromQuery] int agentId)
    {
        var result = await _productService.GetProducts(request, agentId);
        return ResponseCode(result);
    }

    [HttpDelete("delete-agent-product")]
    [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> DeleteProduct(int productId, bool isDigital, int agentId)
    {
        var response = await _productService.DeleteProduct(productId, isDigital, agentId);
        return ResponseCode(response);
    }

    [HttpGet("agent-product-orders")]
    [ProducesResponseType(200, Type = typeof(ApiResponse<IEnumerable<AgentOrderListResponse>>))]
    public async Task<IActionResult> GetProductOrders([FromQuery] AgentProductOrderListRequest request)
    {
        var result = await _agentService.GetAgentProductOrders(request);
        return ResponseCode(result);
    }

    [HttpGet("agent-reviews")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<AgentReview>>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetAgentReviews([FromQuery] AgentReviewRequest request, [FromQuery] int agentId)
    {
        var response = await _productService.GetAgentReviews(request, agentId);
        return ResponseCode(response);
    }
}