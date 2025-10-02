using DaradsHubAPI.Core.Model;
using DaradsHubAPI.Core.Model.Request;
using DaradsHubAPI.Core.Model.Response;
using DaradsHubAPI.Core.Services.Concrete;
using DaradsHubAPI.Core.Services.Interface;
using DaradsHubAPI.WebAPI.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace DaradsHubAPI.WebAPI.Areas.Admin.Controllers;

[Tags("Admin")]
public class ManageCustomerController(IManageCustomerService _manageCustomer) : ApiBaseController
{
    [HttpGet("view-customers-requests")]
    [ProducesResponseType(200, Type = typeof(ApiResponse<IEnumerable<CustomerRequestResponse>>))]
    public async Task<IActionResult> GetCustomerRequestsForAdmin([FromQuery] CustomerRequestsRequest request)
    {
        var result = await _manageCustomer.GetCustomerRequestsForAdmin(request);
        return ResponseCode(result);
    }

    [HttpGet("view-customers-requests-metrics")]
    [ProducesResponseType(200, Type = typeof(ApiResponse<CustomerRequestMetricResponse>))]
    public async Task<IActionResult> GetCustomerRequestMetrics()
    {
        var response = await _manageCustomer.GetCustomerRequestMetricsForAdmin();
        return ResponseCode(response);
    }

    [HttpGet("customers-metrics")]
    [ProducesResponseType(200, Type = typeof(ApiResponse<CustomerMetricsResponse>))]
    public async Task<IActionResult> GetCustomerMetrics()
    {
        var response = await _manageCustomer.GetCustomerMetrics();
        return ResponseCode(response);
    }

    //[HttpGet("top-performing-agents")]
    //[ProducesResponseType(200, Type = typeof(ApiResponse<IEnumerable<TopPerformingAgentResponse>>))]
    //public async Task<IActionResult> TopPerformingAgents()
    //{
    //    var response = await _adminService.TopPerformingAgents();
    //    return ResponseCode(response);
    //}

    //[HttpGet("pending-customer-requests")]
    //[ProducesResponseType(200, Type = typeof(ApiResponse<IEnumerable<LastFourCustomerRequest>>))]
    //public async Task<IActionResult> PendingCustomerRequests()
    //{
    //    var response = await _adminService.PendingCustomerRequests();
    //    return ResponseCode(response);
    //}
}