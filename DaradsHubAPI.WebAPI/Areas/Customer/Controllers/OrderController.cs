using DaradsHubAPI.Core.Model;
using DaradsHubAPI.Core.Model.Request;
using DaradsHubAPI.Core.Model.Response;
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

    [HttpGet("get-carts")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<CartResponse>>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetCart()
    {
        var userId = int.Parse(User.Identity?.GetUserId() ?? "");
        var response = await _orderService.GetCart(userId);
        return ResponseCode(response);
    }

    [HttpPost("add-item-to-cart")]
    [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> AddItemToCart(AddItemToCartRequestModel request)
    {
        var userId = int.Parse(User.Identity?.GetUserId() ?? "");

        var response = await _orderService.AddItemsToCart(request, userId);
        return ResponseCode(response);
    }

    [HttpPost("remove-item-from-cart")]
    [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> RemoveFromCart(AddItemToCartRequestModel request)
    {
        var userId = int.Parse(User.Identity?.GetUserId() ?? "");
        var response = await _orderService.RemoveItemsFromCart(request, userId);
        return ResponseCode(response);
    }

}