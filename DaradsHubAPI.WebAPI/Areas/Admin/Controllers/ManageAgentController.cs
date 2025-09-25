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
public class ManageAgentController(IManageAgentService _agentService) : ApiBaseController
{
    [HttpGet("view-agent-profile")]
    [ProducesResponseType(typeof(ApiResponse<ShortAgentProfileResponse>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetAgentProfile(int agentId)
    {
        var response = await _agentService.GetAgentProfile(agentId);
        return ResponseCode(response);
    }

    [HttpPatch("update-agent-visibility")]
    [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> UpdateAgentVisibility([FromBody] AgentVisibilityRequest request)
    {
        var response = await _agentService.UpdateAgentVisibility(request);
        return ResponseCode(response);
    }

    [HttpPatch("update-agent-status")]
    [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> UpdateAgentStatus([FromBody] AgentStatusRequest request)
    {
        var response = await _agentService.UpdateAgentStatus(request);
        return ResponseCode(response);
    }

    [HttpGet("view-agents")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<AgentsListResponse>>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetAgents([FromQuery] AgentsListRequest request)
    {
        var response = await _agentService.GetAgents(request);
        return ResponseCode(response);

    }

    [HttpPost("create-agent-account")]
    [ProducesResponseType(typeof(ApiResponse<string>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> AddAgent([FromForm] AddAgentRequest request)
    {
        var response = await _agentService.CreateAgent(request);
        return ResponseCode(response);
    }
}