using DaradsHubAPI.Core.Model;
using DaradsHubAPI.Core.Model.Request;
using DaradsHubAPI.Core.Model.Response;
using DaradsHubAPI.Core.Services.Interface;
using DaradsHubAPI.Domain.Entities;
using DaradsHubAPI.WebAPI.Extensions;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace DaradsHubAPI.WebAPI.Areas.Customer.Controllers;

[Tags("Customer")]
public class ProductController(IProductService _productService) : ApiBaseController
{
    [HttpGet("landing-page-products")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<LandingProductResponse>>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetLandPageProducts()
    {
        var response = await _productService.GetLandPageProducts();
        return ResponseCode(response);
    }
    /// <summary>
    /// Rating range is between 1-5
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPost("review-physical-product")]
    [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> AddReview(AddReviewRequestModel model)
    {
        var userId = int.Parse(User.Identity?.GetUserId() ?? "");
        var response = await _productService.AddPhysicalReview(model, userId);
        return ResponseCode(response);
    }

    /// <summary>
    ///  Rating range is between 1-5
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPost("review-agent")]
    [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> AddAgentReview([FromBody] AddAgentReviewRequest model)
    {
        var userId = int.Parse(User.Identity?.GetUserId() ?? "");
        var response = await _productService.AddAgentReview(model, userId);
        return ResponseCode(response);
    }

    [HttpGet("agent-reviews")]
    [ProducesResponseType(typeof(ApiResponse<AgentReviewResponse>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetAgentReviews([FromQuery] int agentId)
    {
        var response = await _productService.GetAgentReviews(agentId);
        return ResponseCode(response);
    }

    [HttpGet("product-reviews")]
    [ProducesResponseType(typeof(ApiResponse<ProductReviewResponse>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetProductReviews([FromQuery] int productId)
    {
        var response = await _productService.GetProductReviews(productId);
        return ResponseCode(response);
    }

    [HttpGet("agent-product-profile")]
    [ProducesResponseType(typeof(ApiResponse<AgentProductProfileResponse>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetAgentProductProfile([FromQuery] int agentId)
    {
        var response = await _productService.GetAgentProductProfile(agentId);
        return ResponseCode(response);
    }

    [HttpGet("agent-products")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<ProductDetailsResponse>>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetAgentProducts([FromQuery] AgentProductListRequest request)
    {
        var response = await _productService.GetAgentProducts(request);
        return ResponseCode(response);
    }

    [HttpGet("agent-product")]
    [ProducesResponseType(typeof(ApiResponse<ProductDetailResponse>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetAgentProduct([FromQuery] int productId)
    {
        var response = await _productService.GetAgentProduct(productId);
        return ResponseCode(response);
    }

    [HttpGet("physical-agents")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<AgentsProfileResponse>>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetPhysicalAgent([FromQuery] AgentsProfileListRequest request)
    {
        var response = await _productService.GetPhysicalAgent(request);
        return ResponseCode(response);
    }

    [HttpPost("create-product-request")]
    [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> CreateProductRequest([FromForm] CreateHubProductRequest request)
    {
        var email = User.Identity?.GetUserEmail() ?? "";
        var response = await _productService.CreateProductRequest(request, email);
        return ResponseCode(response);
    }
}