using DaradsHubAPI.Core.IRepository;
using DaradsHubAPI.Core.Model;
using DaradsHubAPI.Core.Model.Response;
using DaradsHubAPI.Core.Services.Interface;
using static DaradsHubAPI.Domain.Enums.Enum;

namespace DaradsHubAPI.Core.Services.Concrete;
public class ManageAgentService(IUnitOfWork _unitOfWork) : IManageAgentService
{
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
}
