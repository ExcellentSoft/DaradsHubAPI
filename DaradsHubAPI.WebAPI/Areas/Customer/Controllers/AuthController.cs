using DaradsHubAPI.Core.Model;
using DaradsHubAPI.Core.Model.Response;
using DaradsHubAPI.Core.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace DaradsHubAPI.WebAPI.Areas.Customer.Controllers;


[AllowAnonymous]
[Tags("Customer")]
public class AuthController(IAuthService _authService) : ApiBaseController
{
    [HttpPost("create-customer")]
    [ProducesResponseType(typeof(ApiResponse<string>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> SignUp([FromBody] CreateCustomerRequest request)
    {
        var response = await _authService.CreateCustomer(request);
        return ResponseCode(response);
    }
}
