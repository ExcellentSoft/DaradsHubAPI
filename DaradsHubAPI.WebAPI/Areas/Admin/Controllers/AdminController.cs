using DaradsHubAPI.Core.Model;
using DaradsHubAPI.Core.Model.Request;
using DaradsHubAPI.Core.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace DaradsHubAPI.WebAPI.Areas.Admin.Controllers;

[Tags("Admin")]
public class AdminController(IAdminService _adminService) : ApiBaseController
{
    [HttpGet("metrics")]
    [ProducesResponseType(200, Type = typeof(ApiResponse<AdminDashboardMetricResponse>))]
    public async Task<IActionResult> GetDashboardMetrics()
    {
        var response = await _adminService.GetDashboardMetrics();
        return ResponseCode(response);
    }
}
