using DaradsHubAPI.Core.Model;
using DaradsHubAPI.Core.Model.Response;
using DaradsHubAPI.Core.Services.Interface;
using DaradsHubAPI.WebAPI.Extensions;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace DaradsHubAPI.WebAPI.Areas.Agent.Controllers;
[Tags("Agent")]
public class AgentAccountController(IAccountService _accountService) : ApiBaseController
{
    [HttpGet("profile")]
    [ProducesResponseType(typeof(ApiResponse<AgentProfileResponse>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetAgentProfile()
    {
        var email = User.Identity?.GetUserEmail() ?? "";
        var response = await _accountService.GetAgentProfile(email);
        return ResponseCode(response);
    }

    [HttpPatch("update-profile")]
    [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> UpdateProfile([FromForm] AgentProfileRequest request)
    {
        var email = User.Identity?.GetUserEmail() ?? "";
        var response = await _accountService.UpdateAgentProfile(request, email);
        return ResponseCode(response);
    }

    [HttpPatch("change-password")]
    [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        var email = User.Identity?.GetUserEmail() ?? "";
        var response = await _accountService.ChangePassword(request, email);
        return ResponseCode(response);
    }
}