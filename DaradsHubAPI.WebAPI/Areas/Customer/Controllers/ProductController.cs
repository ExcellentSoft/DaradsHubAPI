using DaradsHubAPI.Core.Model;
using DaradsHubAPI.Core.Model.Request;
using DaradsHubAPI.Core.Model.Response;
using DaradsHubAPI.Core.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace DaradsHubAPI.WebAPI.Areas.Customer.Controllers;

[Tags("Customer")]
public class ProductController(IProductService _productService) : ApiBaseController
{
    [HttpGet("landing-page-products")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<LandingProductProductResponse>>), (int)HttpStatusCode.OK)]
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
    [HttpPost("add-review-to-product")]
    [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> AddReview(AddReviewRequestModel model)
    {
        var response = await _productService.AddReview(model);
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
}