using DaradsHubAPI.Core.Model;
using DaradsHubAPI.Core.Model.Request;
using DaradsHubAPI.Core.Model.Response;
using DaradsHubAPI.Core.Services.Concrete;
using DaradsHubAPI.Core.Services.Interface;
using DaradsHubAPI.WebAPI.Extensions;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace DaradsHubAPI.WebAPI.Areas.Admin.Controllers;

[Tags("Admin")]
public class ManageAgentController(IManageAgentService _agentService) : ApiBaseController
{
    [HttpGet("view-agent-profile")]
    [ProducesResponseType(typeof(ApiResponse<ShortAgentProfileResponse>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetAgentProfile(int agentId)
    {
        var response = await _agentService.GetAgentProfile(agentId);
        return ResponseCode(response);
    }
}