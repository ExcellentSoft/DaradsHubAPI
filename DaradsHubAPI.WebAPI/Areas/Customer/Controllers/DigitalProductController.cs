using DaradsHubAPI.Core.Model;
using DaradsHubAPI.Core.Model.Request;
using DaradsHubAPI.Core.Model.Response;
using DaradsHubAPI.Core.Services.Concrete;
using DaradsHubAPI.Core.Services.Interface;
using DaradsHubAPI.WebAPI.Extensions;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace DaradsHubAPI.WebAPI.Areas.Customer.Controllers;

[Tags("Customer")]
public class DigitalProductController(IDigitalProductService _digitalProductService, IProductService _productService) : ApiBaseController
{

    [HttpGet("digital-products-dropdown")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<CategoryResponse>>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetDigitalProducts([FromQuery] string? searchText, [FromQuery] int agentId)
    {
        var response = await _digitalProductService.GetDigitalProducts(searchText, agentId);
        return ResponseCode(response);
    }
    [HttpGet("landing-page-digital-products")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<LandingPageDigitalProductResponse>>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetLandPageProducts()
    {
        var response = await _digitalProductService.GetLandPageProducts();
        return ResponseCode(response);
    }

    /// <summary>
    /// Rating range is between 1-5
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPost("review-digital-product")]
    [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> AddReview(AddReviewRequestModel model)
    {
        var userId = int.Parse(User.Identity?.GetUserId() ?? "");
        var response = await _productService.AddReview(model, userId, true);
        return ResponseCode(response);
    }

    [HttpGet("agent-digital-product-profile")]
    [ProducesResponseType(typeof(ApiResponse<AgentProductProfileResponse>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetAgentDigitalProductProfile([FromQuery] int agentId)
    {
        var response = await _digitalProductService.GetAgentDigitalProductProfile(agentId);
        return ResponseCode(response);
    }

    [HttpGet("agent-digital-products")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<DigitalProductDetailsResponse>>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetAgentProducts([FromQuery] AgentDigitalProductListRequest request)
    {
        var response = await _digitalProductService.GetAgentProducts(request);
        return ResponseCode(response);
    }

    [HttpGet("agent-digital-product")]
    [ProducesResponseType(typeof(ApiResponse<DigitalProductDetailResponse>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetAgentProduct([FromQuery] int productId)
    {
        var response = await _digitalProductService.GetAgentProduct(productId);
        return ResponseCode(response);
    }
}