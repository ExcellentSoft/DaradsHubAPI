using DaradsHubAPI.Core.IRepository;
using DaradsHubAPI.Core.Model;
using DaradsHubAPI.Core.Model.Request;
using DaradsHubAPI.Core.Model.Response;
using DaradsHubAPI.Core.Services.Interface;
using DaradsHubAPI.Domain.Entities;
using DaradsHubAPI.Shared.Extentions;
using DaradsHubAPI.Shared.Interface;
using DaradsHubAPI.Shared.Static;
using Microsoft.EntityFrameworkCore;
using static DaradsHubAPI.Domain.Enums.Enum;

namespace DaradsHubAPI.Core.Services.Concrete;
public class ManageAgentService(IUnitOfWork _unitOfWork, IFileService _fileService) : IManageAgentService
{
    static ApiResponse ValidateAgentRequest(AddAgentRequest request)
    {
        if (string.IsNullOrEmpty(request.FullName))
            return new ApiResponse("First name is required.", StatusEnum.Validation, false);

        else if (string.IsNullOrEmpty(request.Email))
            return new ApiResponse("Email is required.", StatusEnum.Validation, false);

        else if (!request.Email.IsValidEmail())
            return new ApiResponse("Invalid email address.", StatusEnum.Validation, false);

        else if (!request.PhoneNumber.IsValidPhoneNumber())
            return new ApiResponse("Invalid phone number.", StatusEnum.Validation, false);

        else
            return new ApiResponse("Validation passed.", StatusEnum.Success, true);
    }

    public async Task<ApiResponse<string>> CreateAgent(AddAgentRequest request)
    {
        var validateResult = ValidateAgentRequest(request);
        if (!validateResult.Status.GetValueOrDefault())
            return new ApiResponse<string> { Status = validateResult.Status, Message = validateResult.Message, StatusCode = StatusEnum.Validation };

        request.Email = request.Email.Trim().ToLower();

        var customer = await _unitOfWork.Users.GetSingleWhereAsync(u => u.email == request.Email);

        if (await _unitOfWork.Users.AnyAsync(us => us.email == request.Email && us.phone == request.PhoneNumber))
        {
            return new ApiResponse<string> { Status = false, Message = $"Agent with {request.FullName} already exists. Please verify and try again.", StatusCode = StatusEnum.Validation };
        }

        if (_unitOfWork.Users.Any(us => us.email == request.Email))
        {
            return new ApiResponse<string> { Status = false, Message = $"Email  address  {request.Email} already registered, check and try again later.", StatusCode = StatusEnum.Validation };
        }

        if (_unitOfWork.Users.Any(us => us.phone == request.PhoneNumber))
        {

            return new ApiResponse<string> { Status = false, Message = $"Phone number  {request.PhoneNumber} already registered, check and try again later.", StatusCode = StatusEnum.Validation };
        }

        var photoPath = "";
        if (request.Photo is not null)
        {
            var maxUploadSize = 5;
            var allowedExtensions = new[] { ".png", ".jpg", ".jpeg", ".jpe", ".gif" };
            if (request.Photo.Length > (maxUploadSize * 1024 * 1024))
                return new ApiResponse<string> { Status = false, Message = $"Max upload size exceeded. Max size is {maxUploadSize}MB", StatusCode = StatusEnum.Validation };

            var ext = Path.GetExtension(request.Photo.FileName);
            if (!allowedExtensions.Contains(ext))
                return new ApiResponse<string> { Status = false, Message = $"Invalid file format. Supported file formats include {string.Join(", ", allowedExtensions)}", StatusCode = StatusEnum.Validation };

            var fileResponse = await _fileService.AddPhoto(request.Photo, GenericStrings.PROFILE_IMAGES_FOLDER_NAME);
            Uri url = fileResponse.SecureUrl;
            if (!string.IsNullOrEmpty(url.AbsoluteUri))
            {
                photoPath = url.AbsoluteUri;
            }
        }

        var createResponse = await _unitOfWork.Users.AddAgentByAdmin(request, photoPath);
        if (!createResponse.status)
        {
            return new ApiResponse<string> { Status = createResponse.status, Message = createResponse.message, StatusCode = StatusEnum.Validation };
        }

        return new ApiResponse<string> { Status = createResponse.status, Message = createResponse.message, StatusCode = StatusEnum.Success };
    }


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

    public async Task<ApiResponse<bool>> ToggleVisibility(int agentId, bool isPublic)
    {
        var (status, message, _isPublic) = await _unitOfWork.HubUsers.ToggleVisibility(agentId, isPublic);

        if (!status)
        {
            return new ApiResponse<bool> { Message = message, StatusCode = StatusEnum.Validation, Status = status };
        }
        return new ApiResponse<bool> { Message = message, StatusCode = StatusEnum.Success, Status = status, Data = _isPublic };
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

        if (request.EntityStatus == EntityStatusEnum.Suspended)
        {
            var suspended = _unitOfWork.Users.SuspendedAgent(new SuspendedAgent
            {
                AgentId = request.AgentId,
                Duration = request.Duration,
                OptionalNote = request.OptionalNote,
                Reason = request.Reason ?? ""
            });
        }
        if (request.EntityStatus == EntityStatusEnum.Blocked)
        {
            var block = _unitOfWork.Users.BlockAgent(new BlockedAgent
            {
                AgentId = request.AgentId,
                Reason = request.Reason ?? ""
            });
        }
        if (request.EntityStatus == EntityStatusEnum.Active)
        {
            await _unitOfWork.Users.ClearSuspendBlockRecord(request.AgentId);
        }

        return new ApiResponse("Success", StatusEnum.Success, true);
    }

    public async Task<ApiResponse<AgentDashboardMetricResponse>> GetAgentDashboardMetrics(int agentId)
    {
        var responses = await _unitOfWork.Orders.AgentDashboardMetrics(agentId);
        return new ApiResponse<AgentDashboardMetricResponse> { Message = "Successful", Status = true, Data = responses, StatusCode = StatusEnum.Success };
    }

    public async Task<ApiResponse<List<AgentOrderListResponse>>> GetAgentProductOrders(AgentProductOrderListRequest request)
    {
        var response = await _unitOfWork.Products.GetAgentProductOrders(request);
        var totalRecordsCount = response.Count;

        return new ApiResponse<List<AgentOrderListResponse>> { StatusCode = StatusEnum.Success, Message = "Agent Products Orders fetched successfully.", Status = true, Data = response, Pages = request.PageSize, TotalRecord = totalRecordsCount, CurrentPage = request.PageNumber };
    }
}
