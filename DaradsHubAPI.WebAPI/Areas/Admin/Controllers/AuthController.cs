using DaradsHubAPI.Core.Model;
using DaradsHubAPI.Core.Services.Interface;
using DaradsHubAPI.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace DaradsHubAPI.WebAPI.Areas.Admin.Controllers;

[Tags("Admin")]
public class AuthController(IAuthService _authService) : ApiBaseController
{
    [HttpGet("admin-login")]
    [ProducesResponseType(typeof(ApiResponse<CustomerLoginResponse>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> LoginAdmin([FromBody] AdminLoginRequest request)
    {
        var response = await _authService.LoginAdmin(request);
        return ResponseCode(response);
    }
}