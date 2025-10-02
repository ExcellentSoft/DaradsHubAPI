using DaradsHubAPI.Core.Model;
using DaradsHubAPI.Core.Model.Request;
using DaradsHubAPI.Core.Services.Interface;
using DaradsHubAPI.Domain.Entities;
using DaradsHubAPI.WebAPI.Extensions;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace DaradsHubAPI.WebAPI.Areas.Agent.Controllers;


[Tags("Agent")]
public class ManageOrderController(IOrderService _orderService) : ApiBaseController
{
    [HttpGet("order-metrics")]
    [ProducesResponseType(200, Type = typeof(ApiResponse<AgentOrderMetricResponse>))]
    public async Task<IActionResult> OrderMetrics()
    {
        var agentId = int.Parse(User.Identity?.GetUserId() ?? "");
        var response = await _orderService.OrderMetrics(agentId);
        return ResponseCode(response);
    }

    [HttpGet("orders")]
    [ProducesResponseType(200, Type = typeof(ApiResponse<IEnumerable<AgentOrderListResponse>>))]
    public async Task<IActionResult> GetOrders([FromQuery] AgentOrderListRequest request)
    {
        var agentId = int.Parse(User.Identity?.GetUserId() ?? "");
        var result = await _orderService.GetOrders(request, agentId);
        return ResponseCode(result);
    }

    [HttpPut("change-order-status")]
    [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> ChangeOrderStatus([FromQuery] ChangeStatusRequest request)
    {
        var response = await _orderService.ChangeOrderStatus(request);
        return ResponseCode(response);
    }

    [HttpGet("order")]
    [ProducesResponseType(200, Type = typeof(ApiResponse<SingleOrderResponse>))]
    public async Task<IActionResult> GetOrder([FromQuery] string orderCode)
    {
        var result = await _orderService.GetOrder(orderCode);
        return ResponseCode(result);
    }

    [HttpGet("agent-customers")]
    [ProducesResponseType(200, Type = typeof(ApiResponse<IEnumerable<AgentCustomerOrderResponse>>))]
    public async Task<IActionResult> GetAgentCustomersOrders([FromQuery] AgentCustomerRequest request)
    {
        var agentId = int.Parse(User.Identity?.GetUserId() ?? "");
        var result = await _orderService.GetAgentCustomersOrders(request, agentId);
        return ResponseCode(result);
    }
}