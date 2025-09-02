using DaradsHubAPI.Core.Model;
using DaradsHubAPI.Core.Model.Response;
using DaradsHubAPI.Core.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace DaradsHubAPI.WebAPI.Areas.Agent.Controllers;

[AllowAnonymous]
[Tags("Agent")]
public class AuthController(IAuthService _authService) : ApiBaseController
{
    [HttpPost("create-agent")]
    [ProducesResponseType(typeof(ApiResponse<string>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> SignUp([FromBody] CreateAgentRequest request)
    {
        var response = await _authService.CreateAgent(request);
        return ResponseCode(response);
    }
}
