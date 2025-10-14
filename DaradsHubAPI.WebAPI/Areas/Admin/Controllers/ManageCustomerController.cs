using DaradsHubAPI.Core.Model;
using DaradsHubAPI.Core.Model.Request;
using DaradsHubAPI.Core.Model.Response;
using DaradsHubAPI.Core.Services.Concrete;
using DaradsHubAPI.Core.Services.Interface;
using DaradsHubAPI.Domain.Entities;
using DaradsHubAPI.WebAPI.Extensions;
using Microsoft.AspNetCore.Mvc;
using System.Net;

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

    [HttpGet("customers")]
    [ProducesResponseType(200, Type = typeof(ApiResponse<IEnumerable<CustomersListResponse>>))]
    public async Task<IActionResult> GetCustomers([FromQuery] CustomersListRequest request)
    {
        var response = await _manageCustomer.GetCustomers(request);
        return ResponseCode(response);
    }

    [HttpGet("view-customer-profile")]
    [ProducesResponseType(typeof(ApiResponse<ShortCustomerProfileResponse>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetCustomerProfile(int customerId)
    {
        var response = await _manageCustomer.GetCustomerProfile(customerId);
        return ResponseCode(response);
    }

    [HttpPatch("update-customer-status")]
    [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> UpdateAgentStatus([FromBody] CustomerStatusRequest request)
    {
        var response = await _manageCustomer.UpdateCustomerStatus(request);
        return ResponseCode(response);
    }

    [HttpGet("customer-recent-orders")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<OrderListResponse>>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetRecentOrders([FromQuery] OrderListRequest request, [FromQuery] string email)
    {
        var response = await _manageCustomer.GetRecentOrders(email, request);
        return ResponseCode(response);
    }
}