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
    public async Task<IActionResult> AddItemToCart([FromBody] AddItemToCartRequestModel request)
    {
        var userId = int.Parse(User.Identity?.GetUserId() ?? "");

        var response = await _orderService.AddItemsToCart(request, userId);
        return ResponseCode(response);
    }

    [HttpPost("remove-item-from-cart")]
    [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> RemoveFromCart([FromBody] AddItemToCartRequestModel request)
    {
        var userId = int.Parse(User.Identity?.GetUserId() ?? "");
        var response = await _orderService.RemoveItemsFromCart(request, userId);
        return ResponseCode(response);
    }

    [HttpPost("add-shipping-address")]
    [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> AddShippingAddress([FromBody] ShippingAddressRequest request)
    {
        var userId = int.Parse(User.Identity?.GetUserId() ?? "");
        var response = await _orderService.AddShippingAddress(request, userId);
        return ResponseCode(response);
    }

    [HttpDelete("remove-shipping-address")]
    [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> RemoveShippingAddress([FromQuery] long addressId)
    {
        var userId = int.Parse(User.Identity?.GetUserId() ?? "");
        var response = await _orderService.RemoveShippingAddress(userId, addressId);
        return ResponseCode(response);
    }

    [HttpGet("get-shipping-addresses")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<ShippingAddressResponse>>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetShippingAddress()
    {
        var userId = int.Parse(User.Identity?.GetUserId() ?? "");
        var response = await _orderService.GetShippingAddress(userId);
        return ResponseCode(response);
    }

    [HttpPost("checkout-physical-products")]
    [ProducesResponseType(typeof(ApiResponse<string>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> Checkout([FromBody] CheckoutRequest request)
    {
        var email = User.Identity?.GetUserEmail() ?? "";
        var userId = int.Parse(User.Identity?.GetUserId() ?? "");
        var response = await _orderService.CheckOut(request, email, userId);

        return ResponseCode(response);
    }

    [HttpPost("checkout-digital-product")]
    [ProducesResponseType(typeof(ApiResponse<DigitalCheckoutResponse>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> CheckoutDigital([FromBody] CheckoutDigitalRequest request)
    {
        var email = User.Identity?.GetUserEmail() ?? "";
        var response = await _orderService.CheckOutDigital(request, email);

        return ResponseCode(response);
    }
}