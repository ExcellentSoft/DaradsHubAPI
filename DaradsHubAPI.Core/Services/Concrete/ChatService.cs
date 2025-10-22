using DaradsHubAPI.Core.IRepository;
using DaradsHubAPI.Core.Model;
using DaradsHubAPI.Core.Model.Request;
using DaradsHubAPI.Core.Services.Interface;
using DaradsHubAPI.Domain.Entities;
using DaradsHubAPI.Shared.Customs;
using Microsoft.EntityFrameworkCore;
using static DaradsHubAPI.Domain.Enums.Enum;

namespace DaradsHubAPI.Core.Services.Concrete;
public class ChatService(IUnitOfWork _unitOfWork, IEmailService _emailService) : IChatService
{
    public async Task<ApiResponse<IEnumerable<OnlineAgentsResponse>>> GetOnlineAgents(string? searchText)
    {
        searchText = searchText?.Trim().ToLower();
        var onlineAgents = _unitOfWork.Users.
            GetWhere(user => user.IsAgent == true && user.IsOnline == true && (searchText == null || user.fullname.ToLower().Contains(searchText))).Select(c => new OnlineAgentsResponse
            {
                FullName = c.fullname,
                Photo = c.Photo,
                userId = c.id
            }).ToList();

        return await Task.FromResult(new ApiResponse<IEnumerable<OnlineAgentsResponse>> { Data = onlineAgents, Message = "Successful", Status = true, StatusCode = StatusEnum.Success });
    }

    public async Task<ApiResponse<HubChatConversation>> CreateConversation(CreateConversationRequest request)
    {
        var conversation = await _unitOfWork.Notifications.GetOrCreateConversation(request);
        return new ApiResponse<HubChatConversation> { Data = conversation, Message = "Successful", Status = true, StatusCode = StatusEnum.Success };
    }
    public async Task<ApiResponse<HubChatConversation>> GetOrCreateAdminConversationWithAgent(CreateAdminConversationWithAgentRequest request)
    {
        var conversation = await _unitOfWork.Notifications.GetOrCreateAdminConversationWithAgent(request);
        return new ApiResponse<HubChatConversation> { Data = conversation, Message = "Successful", Status = true, StatusCode = StatusEnum.Success };
    }
    public async Task<ApiResponse<HubChatConversation>> GetOrCreateAdminConversationWithCustomer(CreateAdminConversationWithCustomerRequest request)
    {
        var conversation = await _unitOfWork.Notifications.GetOrCreateAdminConversationWithCustomer(request);
        return new ApiResponse<HubChatConversation> { Data = conversation, Message = "Successful", Status = true, StatusCode = StatusEnum.Success };
    }

    public async Task<ApiResponse> ChangeAgentOnlineStatus(bool isOnline, int agentId)
    {
        var agent = await _unitOfWork.Users.GetSingleWhereAsync(u => u.id == agentId && u.IsAgent == true);
        if (agent is null)
        {
            return new ApiResponse("Agent record not found", StatusEnum.Validation, false);
        }

        agent.IsOnline = isOnline;
        agent.ModifiedDate = GetLocalDateTime.CurrentDateTime();
        await _unitOfWork.Users.SaveAsync();

        return new ApiResponse("Success", StatusEnum.Success, true);
    }

    public async Task<ApiResponse> JoinConversation(JoinConversationRequest request)
    {
        var (status, message) = await _unitOfWork.Notifications.JoinConversation(request);
        if (!status)
        {
            return new ApiResponse(message, StatusEnum.Validation, false);
        }

        return new ApiResponse("Success", StatusEnum.Success, true);
    }

    public async Task<ApiResponse> ReportAgent(ReportAgentRequest request)
    {
        if (request.AgentId <= 0)
            return new ApiResponse("Agent is required", StatusEnum.Validation, false);

        var report = new ReportAgent
        {
            Reason = request.Reason,
            AgentId = request.AgentId,
            ReportedDate = GetLocalDateTime.CurrentDateTime(),
            CustomerId = request.CustomerId
        };

        await _unitOfWork.Notifications.ReportAgent(report);

        return new ApiResponse("Report created successfully.", StatusEnum.Success, true);
    }

    public async Task<ApiResponse<IEnumerable<ChatMessageResponse>>> GetChatMessages(MessageListRequest request)
    {
        var messages = _unitOfWork.Notifications.GetChatMessages(request.ConversationId);

        var totalRecordsCount = messages.Count();
        var iMessages = await messages.Skip((request!.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToListAsync();
        return new ApiResponse<IEnumerable<ChatMessageResponse>> { Data = iMessages, Message = "Messages fetched successfully.", Status = true, StatusCode = StatusEnum.Success, Pages = request.PageSize, TotalRecord = totalRecordsCount, CurrentPageCount = request.PageNumber };
    }

    public async Task GetUnreadChatMessages()
    {
        var messages = _unitOfWork.Notifications.GetUnreadChatMessages();
        var emailMessages = $"Hello Agent! please log in to the Darads portal immediately. Customer messages are awaiting your response.";
        if (messages.Any())
        {
            var email = string.Join(',', messages.Select(e => e.Email));
            await _emailService.SendMail(email, "Notice", emailMessages, "Darads", useTemplate: true);
        }
    }

    public async Task<ApiResponse<IEnumerable<ViewChatMessagesResponse>>> GetAgentChatMessages(int agentId)
    {
        var messages = await _unitOfWork.Notifications.GetAgentChatMessages(agentId).ToListAsync();

        return new ApiResponse<IEnumerable<ViewChatMessagesResponse>> { Data = messages, Message = "Agent chats  fetched successfully.", Status = true, StatusCode = StatusEnum.Success };
    }

    public async Task<ApiResponse<IEnumerable<ViewChatMessagesResponse>>> GetChatMessages()
    {
        var messages = await _unitOfWork.Notifications.GetChatMessages().ToListAsync();

        return new ApiResponse<IEnumerable<ViewChatMessagesResponse>> { Data = messages, Message = "Chats Messages fetched successfully.", Status = true, StatusCode = StatusEnum.Success };
    }

    public async Task<ApiResponse<IEnumerable<ViewChatMessagesResponse>>> GetCustomerChatMessages(int customerId)
    {
        var messages = await _unitOfWork.Notifications.GetCustomerChatMessages(customerId).ToListAsync();

        return new ApiResponse<IEnumerable<ViewChatMessagesResponse>> { Data = messages, Message = "Customer chats  fetched successfully.", Status = true, StatusCode = StatusEnum.Success };
    }

    public async Task<ApiResponse<IEnumerable<ViewChatMessagesResponse>>> GetAdminChatMessages(int adminId)
    {
        var messages = await _unitOfWork.Notifications.GetAdminChatMessages(adminId).ToListAsync();

        return new ApiResponse<IEnumerable<ViewChatMessagesResponse>> { Data = messages, Message = "Admin chats  fetched successfully.", Status = true, StatusCode = StatusEnum.Success };
    }

    public async Task<ApiResponse> MarkAllMessageAsRead(long conversationId)
    {
        await _unitOfWork.Notifications.MarkAllMessageAsRead(conversationId);
        return new ApiResponse("Success.", StatusEnum.Success, true);
    }

    public async Task AddMessageAsync(long conversationId, int senderId, string message)
    {
        var msg = new HubChatMessage
        {
            ConversationId = conversationId,
            SenderId = senderId,
            Content = message,
            SentAt = GetLocalDateTime.CurrentDateTime(),
            IsRead = false
        };
        await _unitOfWork.Notifications.SaveChatMessage(msg);
    }
}
