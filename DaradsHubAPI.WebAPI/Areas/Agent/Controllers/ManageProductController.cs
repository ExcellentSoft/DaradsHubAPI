using DaradsHubAPI.Core.Model;
using DaradsHubAPI.Core.Model.Request;
using DaradsHubAPI.Core.Model.Response;
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
}