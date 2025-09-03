using DaradsHubAPI.Core.Model;
using DaradsHubAPI.Core.Model.Request;
using DaradsHubAPI.Core.Model.Response;
using DaradsHubAPI.Core.Services.Interface;
using DaradsHubAPI.WebAPI.Extensions;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace DaradsHubAPI.WebAPI.Areas.Agent.Controllers;
[Tags("Agent")]
public class ManageProductController(IProductService _productService) : ApiBaseController
{
    [HttpPost("add-product")]
    [ProducesResponseType(typeof(ApiResponse<AgentProfileResponse>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> AddProduct([FromForm] AddAgentHubProductRequest request)
    {
        var email = User.Identity?.GetUserEmail() ?? "";
        var response = await _productService.AddProduct(request, email);
        return ResponseCode(response);
    }

    [HttpPatch("update-product")]
    [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> UpdateProfile([FromForm] UpdateAgentHubProductRequest request)
    {
        var email = User.Identity?.GetUserEmail() ?? "";
        var response = await _productService.UpdateProduct(request, email);
        return ResponseCode(response);
    }
}