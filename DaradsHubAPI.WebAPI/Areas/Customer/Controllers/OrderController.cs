using DaradsHubAPI.Core.Model;
using DaradsHubAPI.Core.Services.Concrete;
using DaradsHubAPI.Core.Services.Interface;
using DaradsHubAPI.Domain.Entities;
using DaradsHubAPI.WebAPI.Extensions;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace DaradsHubAPI.WebAPI.Areas.Customer.Controllers;

[Tags("Customer")]
public class OrderController(IOrderService _orderService) : ApiBaseController
{
    [HttpGet("orders")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<OrderListResponse>>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetAgentProducts([FromQuery] OrderListRequest request)
    {
        var email = User.Identity?.GetUserEmail() ?? "";
        var response = await _orderService.GetOrders(email, request);
        return ResponseCode(response);
    }
}