using DaradsHubAPI.Core.Model;
using DaradsHubAPI.Core.Model.Request;
using DaradsHubAPI.Core.Model.Response;
using DaradsHubAPI.Core.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace DaradsHubAPI.WebAPI.Controllers;


[AllowAnonymous]
[Tags("Utilities")]
public class UtilitiesController(ICategoryService _categoryService, IProductService _productService) : ApiBaseController
{

    [HttpGet("agents-dropdown")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<IdNameRecord>>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetAgentsLookUp([FromQuery] string? searchText)
    {
        var response = await _categoryService.GetAgentsLookUp(searchText);
        return ResponseCode(response);
    }

    [HttpGet("products-dropdown")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<CategoryResponse>>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetHubProducts([FromQuery] string? searchText)
    {
        var response = await _productService.GetHubProducts(searchText);
        return ResponseCode(response);
    }

    [HttpGet("categories-dropdown")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<CategoryResponse>>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetCategories([FromQuery] string? searchText)
    {
        var response = await _categoryService.GetCategories(searchText);
        return ResponseCode(response);
    }

    [HttpGet("sub-categories-dropdown")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<IdNameRecord>>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetSubCategories([FromQuery] string? searchText, [FromQuery] int categoryId)
    {
        var response = await _categoryService.GetSubCategories(searchText, categoryId);
        return ResponseCode(response);
    }

    [AllowAnonymous]
    [HttpGet("faq")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<HubFAQResponse>>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetFAQs([FromQuery] string? searchText)
    {
        var response = await _productService.GetFAQs(searchText);
        return ResponseCode(response);
    }
}