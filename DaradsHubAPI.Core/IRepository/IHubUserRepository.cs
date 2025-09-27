using DaradsHubAPI.Core.Model.Request;
using DaradsHubAPI.Core.Model.Response;
using DaradsHubAPI.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaradsHubAPI.Core.IRepository;
public interface IHubUserRepository : IGenericRepository<userstb>
{
    Task<(bool status, string message, ShortAgentProfileResponse? res)> GetAgentProductProfile(int agentId);
    IQueryable<AgentsListResponse> GetAgents(AgentsListRequest request);
    Task<(bool status, string message, bool IsPublic)> ToggleVisibility(int agentId, bool IsPublic);
    Task<(bool status, string message)> UpdateAgentVisibility(AgentVisibilityRequest request);
}
