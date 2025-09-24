using DaradsHubAPI.Core.Model;
using DaradsHubAPI.Core.Model.Request;
using DaradsHubAPI.Core.Model.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaradsHubAPI.Core.Services.Interface;
public interface IManageAgentService
{
    Task<ApiResponse<ShortAgentProfileResponse>> GetAgentProfile(int agentId);
    Task<ApiResponse> UpdateAgentStatus(AgentStatusRequest request);
    Task<ApiResponse> UpdateAgentVisibility(AgentVisibilityRequest request);
}
