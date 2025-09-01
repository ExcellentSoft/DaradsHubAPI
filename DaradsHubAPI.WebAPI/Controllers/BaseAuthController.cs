using DaradsHubAPI.Core.Model;
using DaradsHubAPI.Core.Model.Response;
using DaradsHubAPI.Core.Services.Interface;
using DaradsHubAPI.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace DaradsHubAPI.WebAPI.Controllers;


[AllowAnonymous]
[Tags("BaseAuth")]
public class BaseAuthController(IAuthService _authService) : ApiBaseController
{
    [HttpPost("login")]
    [ProducesResponseType(typeof(ApiResponse<CustomerLoginResponse>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var response = await _authService.LoginUser(request);
        return ResponseCode(response);
    }

    [HttpPost("verify-user-email/{code}")]
    [ProducesResponseType(typeof(ApiResponse<CustomerLoginResponse>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> VerifyCustomerEmail([FromRoute] string code)
    {
        var response = await _authService.VerifyUserAccount(code);
        return ResponseCode(response);
    }

    [HttpPost("resend-email-verificationCode/{userId}")]
    [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> ResendEmailVerificationCode([FromRoute] string userId)
    {
        var response = await _authService.ResendEmailVerificationCode(userId);
        return ResponseCode(response);
    }

    [HttpPost("forget-password")]
    [ProducesResponseType(typeof(ApiResponse<string>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> ForgetPassword([FromBody] ForgetPasswordRequest request)
    {
        var response = await _authService.ForgetPassword(request);
        return ResponseCode(response);
    }

    [HttpPost("reset-password")]
    [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
    {
        var response = await _authService.ResetPassword(request);
        return ResponseCode(response);
    }

    [HttpPost("resend-forget-passwordCode/{email}")]
    [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> ResendResetPasswordCode([FromRoute] string email)
    {
        var response = await _authService.ResendResetPasswordCode(email);
        return ResponseCode(response);
    }
}