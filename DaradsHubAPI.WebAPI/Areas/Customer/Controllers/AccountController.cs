using DaradsHubAPI.Core.Model;
using DaradsHubAPI.Core.Model.Response;
using DaradsHubAPI.Core.Services.Interface;
using DaradsHubAPI.WebAPI.Extensions;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace DaradsHubAPI.WebAPI.Areas.Customer.Controllers;
[Tags("Customer")]
public class AccountController(IAccountService _accountService) : ApiBaseController
{
    [HttpGet("profile")]
    [ProducesResponseType(typeof(ApiResponse<CustomerProfileResponse>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetProfile()
    {
        var email = User.Identity?.GetUserEmail() ?? "";
        var response = await _accountService.GetCustomerProfile(email);
        return ResponseCode(response);
    }

    [HttpPatch("update-profile")]
    [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> UpdateProfile([FromForm] CustomerProfileRequest request)
    {
        var email = User.Identity?.GetUserEmail() ?? "";
        var response = await _accountService.UpdateProfile(request, email);
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