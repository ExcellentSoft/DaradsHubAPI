using DaradsHubAPI.Core.Model;
using DaradsHubAPI.Core.Model.Request;
using DaradsHubAPI.Core.Model.Response;
using DaradsHubAPI.Core.Services.Concrete;
using DaradsHubAPI.Core.Services.Interface;
using DaradsHubAPI.Domain.Entities;
using DaradsHubAPI.WebAPI.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace DaradsHubAPI.WebAPI.Areas.Customer.Controllers;

[Tags("Customer")]
public class DigitalProductController(IDigitalProductService _digitalProductService, IProductService _productService) : ApiBaseController
{
    /// <summary>
    /// Get agent catalogues 
    /// </summary>
    /// <param name="searchText"></param>
    /// <param name="agentId"></param>
    /// <returns></returns>
    [AllowAnonymous]

    [HttpGet("digital-products-dropdown")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<IdNameRecord>>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetDigitalProducts([FromQuery] string? searchText, [FromQuery] int agentId)
    {
        var response = await _digitalProductService.GetDigitalProducts(searchText, agentId);
        return ResponseCode(response);
    }

    /// <summary>
    /// Get agent categories
    /// </summary>
    /// <param name="searchText"></param>
    /// <param name="agentId"></param>
    /// <returns></returns>

    [AllowAnonymous]
    [HttpGet("agent-categories-dropdown")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<IdNameRecord>>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetAgentCategories([FromQuery] string? searchText, [FromQuery] int agentId)
    {
        var response = await _productService.GetAgentCategories(searchText, agentId);
        return ResponseCode(response);
    }

    [AllowAnonymous]
    [HttpGet("landing-page-digital-products")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<LandingPageDigitalProductResponse>>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetLandPageProducts()
    {
        bool isAuthenticate = User.Identity!.IsAuthenticated;
        if (isAuthenticate)
        {
            var response = await _digitalProductService.GetLandPageProducts();
            return ResponseCode(response);
        }
        else
        {
            var response = await _digitalProductService.GetPublicLandPageProducts();
            return ResponseCode(response);
        }

    }

    /// <summary>
    /// Rating range is between 1-5
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPost("review-digital-product")]
    [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> AddDigitalReview(AddReviewRequestModel model)
    {
        var userId = int.Parse(User.Identity?.GetUserId() ?? "");
        var response = await _productService.AddDigitalReview(model, userId);
        return ResponseCode(response);
    }

    [AllowAnonymous]
    [HttpGet("agent-digital-product-profile")]
    [ProducesResponseType(typeof(ApiResponse<AgentProductProfileResponse>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetAgentDigitalProductProfile([FromQuery] int agentId)
    {
        bool isAuthenticate = User.Identity!.IsAuthenticated;
        if (isAuthenticate)
        {
            var response = await _digitalProductService.GetAgentDigitalProductProfile(agentId);
            return ResponseCode(response);
        }
        else
        {
            var response = await _digitalProductService.GetPublicAgentDigitalProductProfile(agentId);
            return ResponseCode(response);
        }
    }

    [AllowAnonymous]
    [HttpGet("similar-digital-products")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<SimilarProductResponse>>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetSimilarDigitalProducts([FromQuery] long productId)
    {
        var response = await _digitalProductService.GetSimilarDigitalProducts(productId);
        return ResponseCode(response);
    }

    [AllowAnonymous]
    [HttpGet("agent-digital-products")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<DigitalProductDetailsResponse>>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetAgentProducts([FromQuery] AgentDigitalProductListRequest request)
    {
        bool isAuthenticate = User.Identity!.IsAuthenticated;
        if (isAuthenticate)
        {
            var response = await _digitalProductService.GetAgentProducts(request);
            return ResponseCode(response);
        }
        else
        {
            var response = await _digitalProductService.GetPublicAgentProducts(request);
            return ResponseCode(response);
        }
    }

    [AllowAnonymous]
    [HttpGet("agent-digital-product")]
    [ProducesResponseType(typeof(ApiResponse<DigitalProductDetailResponse>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetAgentProduct([FromQuery] int productId)
    {
        var response = await _digitalProductService.GetAgentProduct(productId);
        return ResponseCode(response);
    }

    [AllowAnonymous]
    [HttpGet("digital-agents")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<AgentsProfileResponse>>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetDigitalAgents([FromQuery] AgentsProfileListRequest request)
    {
        bool isAuthenticate = User.Identity!.IsAuthenticated;
        if (isAuthenticate)
        {
            var response = await _digitalProductService.GetDigitalAgents(request);
            return ResponseCode(response);
        }
        else
        {
            var response = await _digitalProductService.GetDigitalPublicAgents(request);
            return ResponseCode(response);
        }
    }
}