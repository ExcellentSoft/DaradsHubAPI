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
public class ManageProductController(IProductService _productService, IDigitalProductService _digitalProductService) : ApiBaseController
{
    [HttpPost("add-product")]
    [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> AddProduct([FromForm] AddAgentHubProductRequest request)
    {
        var email = User.Identity?.GetUserEmail() ?? "";
        var response = await _productService.AddProduct(request, email);
        return ResponseCode(response);
    }

    [HttpPut("update-product")]
    [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> UpdateProfile([FromForm] UpdateAgentHubProductRequest request)
    {
        var email = User.Identity?.GetUserEmail() ?? "";
        var response = await _productService.UpdateProduct(request, email);
        return ResponseCode(response);
    }

    [HttpPost("add-digital-product")]
    [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> AddDigitalProduct([FromForm] AddDigitalHubProductRequest request)
    {
        var email = User.Identity?.GetUserEmail() ?? "";
        var response = await _digitalProductService.AddDigitalProduct(request, email);
        return ResponseCode(response);
    }

    [HttpPut("update-digital-product")]
    [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> UpdateProfile([FromForm] UpdateDigitalHubProductRequest request)
    {
        var email = User.Identity?.GetUserEmail() ?? "";
        var response = await _digitalProductService.UpdateDigitalProduct(request, email);
        return ResponseCode(response);
    }

    [HttpGet("product-metrics")]
    [ProducesResponseType(200, Type = typeof(ApiResponse<ProductMetricResponse>))]
    public async Task<IActionResult> GetProductMetrics()
    {
        var agentId = int.Parse(User.Identity?.GetUserId() ?? "");
        var response = await _productService.GetProductMetrics(agentId);
        return ResponseCode(response);
    }

    [HttpDelete("delete-product")]
    [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> UpdateProfile(int productId, bool isDigital)
    {
        var agentId = int.Parse(User.Identity?.GetUserId() ?? "");
        var response = await _productService.DeleteProduct(productId, isDigital, agentId);
        return ResponseCode(response);
    }

    [HttpGet("products")]
    [ProducesResponseType(200, Type = typeof(ApiResponse<IEnumerable<AgentProductsResponse>>))]
    public async Task<IActionResult> GetProducts([FromQuery] AgentProductsRequest request)
    {
        var agentId = int.Parse(User.Identity?.GetUserId() ?? "");
        var result = await _productService.GetProducts(request, agentId);
        return ResponseCode(result);
    }

    [HttpGet("product-ordered-metrics")]
    [ProducesResponseType(200, Type = typeof(ApiResponse<ProductOrderMetricResponse>))]
    public async Task<IActionResult> GetProductOrderMetrics(long productId, bool isDigital)
    {
        var response = await _productService.GetProductOrderMetrics(productId, isDigital);
        return ResponseCode(response);
    }

    [HttpGet("product-orders")]
    [ProducesResponseType(200, Type = typeof(ApiResponse<IEnumerable<AgentOrderListResponse>>))]
    public async Task<IActionResult> GetProductOrders([FromQuery] ProductOrderListRequest request)
    {
        var result = await _productService.GetProductOrders(request);
        return ResponseCode(result);
    }

    [HttpGet("customers-requests")]
    [ProducesResponseType(200, Type = typeof(ApiResponse<IEnumerable<CustomerRequestResponse>>))]
    public async Task<IActionResult> GetCustomerRequests([FromQuery] CustomerRequestsRequest request)
    {
        var agentId = int.Parse(User.Identity?.GetUserId() ?? "");
        var result = await _productService.GetCustomerRequests(request, agentId);
        return ResponseCode(result);
    }

    [HttpGet("customers-request")]
    [ProducesResponseType(200, Type = typeof(ApiResponse<SingleCustomerRequestResponse>))]
    public async Task<IActionResult> GetCustomerRequest([FromQuery] long requestId)
    {
        var result = await _productService.GetCustomerRequest(requestId);
        return ResponseCode(result);
    }

    [HttpPut("change-request-status")]
    [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> ChangeRequestStatus([FromQuery] ChangeRequestStatus request)
    {
        var response = await _productService.ChangeRequestStatus(request);
        return ResponseCode(response);
    }

    [HttpGet("reviews")]
    [ProducesResponseType(typeof(ApiResponse<AgentReview>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetAgentReviews([FromQuery] AgentReviewRequest request)
    {
        var agentId = int.Parse(User.Identity?.GetUserId() ?? "");
        var response = await _productService.GetAgentReviews(request, agentId);
        return ResponseCode(response);
    }
}