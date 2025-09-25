using DaradsHubAPI.Core.IRepository;
using DaradsHubAPI.Core.Model;
using DaradsHubAPI.Core.Model.Request;
using DaradsHubAPI.Core.Model.Response;
using DaradsHubAPI.Core.Services.Interface;
using DaradsHubAPI.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using static DaradsHubAPI.Domain.Enums.Enum;

namespace DaradsHubAPI.Core.Services.Concrete;
public class ManageAgentService(IUnitOfWork _unitOfWork) : IManageAgentService
{
    public async Task<ApiResponse<IEnumerable<AgentsListResponse>>> GetAgents(AgentsListRequest request)
    {
        var query = _unitOfWork.HubUsers.GetAgents(request);

        var totalProducts = query.Count();
        var paginatedAgents = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize).ToListAsync();

        return new ApiResponse<IEnumerable<AgentsListResponse>> { Message = "Successful", Status = true, Data = paginatedAgents, StatusCode = StatusEnum.Success, TotalRecord = totalProducts, Pages = request.PageSize, CurrentPageCount = request.PageNumber };
    }

    public async Task<ApiResponse<ShortAgentProfileResponse>> GetAgentProfile(int agentId)
    {
        var profileResponse = await _unitOfWork.HubUsers.GetAgentProductProfile(agentId);

        if (!profileResponse.status)
        {
            if (!profileResponse.status)
            {
                return new ApiResponse<ShortAgentProfileResponse> { Status = profileResponse.status, Message = profileResponse.message, StatusCode = StatusEnum.Validation };
            }
        }
        return new ApiResponse<ShortAgentProfileResponse> { Status = profileResponse.status, Message = profileResponse.message, StatusCode = StatusEnum.Success, Data = profileResponse.res ?? new ShortAgentProfileResponse { } };
    }

    public async Task<ApiResponse> UpdateAgentVisibility(AgentVisibilityRequest request)
    {
        var (status, message) = await _unitOfWork.HubUsers.UpdateAgentVisibility(request);

        if (!status)
        {
            return new ApiResponse(message, StatusEnum.Validation, status);
        }
        return new ApiResponse(message, StatusEnum.Success, status);
    }

    public async Task<ApiResponse> UpdateAgentStatus(AgentStatusRequest request)
    {
        var agent = await _unitOfWork.HubUsers.GetSingleWhereAsync(d => d.id == request.AgentId);

        if (agent is null)
        {
            return new ApiResponse("Agent record not found.", StatusEnum.Validation, false);
        }
        var user = await _unitOfWork.Users.GetAppUser(agent.email);
        if (user is null)
        {
            return new ApiResponse("User record not found.", StatusEnum.Validation, false);
        }

        user.Status = request.EntityStatus;

        agent.status = (int)request.EntityStatus;

        await _unitOfWork.Users.SaveAsync();

        return new ApiResponse("Success", StatusEnum.Success, true);
    }
}
