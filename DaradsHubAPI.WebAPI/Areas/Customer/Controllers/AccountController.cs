using Darads.CoreInfrastruture.Persistence.IIntegration;
using DaradsHubAPI.Core.Model;
using DaradsHubAPI.Core.Model.Response;
using DaradsHubAPI.Core.Services.Interface;
using DaradsHubAPI.WebAPI.Extensions;
using Microsoft.AspNetCore.Mvc;
using System.Net;
// private readonly IPaymentIOService _paymentIoService;
namespace DaradsHubAPI.WebAPI.Areas.Customer.Controllers;
[Tags("Customer")]
public class AccountController(IAccountService _accountService, IPaymentIOService _paymentIoService) : ApiBaseController
{
    [HttpGet("dashboard-metrics")]
    [ProducesResponseType(typeof(ApiResponse<DashboardMetricsResponse>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> DashboardMetrics()
    {
        var email = User.Identity?.GetUserEmail() ?? "";
        var response = await _accountService.DashboardMetrics(email);
        return ResponseCode(response);
    }

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

    [HttpPatch("Submit-PaymentDetails")]
    [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> SubmitPayD([FromBody] SubmitCashPayRequest request)
    {
        var email = User.Identity?.GetUserEmail() ?? "";
        request.UserId = email;
        var response = await _accountService.SubmitCashPay(request);
        return ResponseCode(response);
    }
    
    [HttpPatch("Create-New-VIA")]
    [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> CreateNewVirtualAccount(CreateVirtualParam param)
    {
        var email = User.Identity?.GetUserEmail() ?? "";

        var res = await _paymentIoService.CreateVirtualAccount(param.userEmail, "WB");
        return ResponseCode(res);
    }

     
}