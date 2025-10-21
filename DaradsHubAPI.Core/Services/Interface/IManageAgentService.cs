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
    Task<ApiResponse<string>> CreateAgent(AddAgentRequest request);
    Task<ApiResponse<AgentDashboardMetricResponse>> GetAgentDashboardMetrics(int agentId);
    Task<ApiResponse<List<AgentOrderListResponse>>> GetAgentProductOrders(AgentProductOrderListRequest request);
    Task<ApiResponse<ShortAgentProfileResponse>> GetAgentProfile(int agentId);
    Task<ApiResponse<IEnumerable<AgentsListResponse>>> GetAgents(AgentsListRequest request);
    Task<ApiResponse<IEnumerable<ReportedAgentsResponse>>> GetReportedAgents();
    Task<ApiResponse<bool>> ToggleVisibility(int agentId, bool isPublic);
    Task<ApiResponse> UpdateAgentStatus(AgentStatusRequest request);
    Task<ApiResponse> UpdateAgentVisibility(AgentVisibilityRequest request);
}
