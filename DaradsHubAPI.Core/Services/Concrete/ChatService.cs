using DaradsHubAPI.Core.IRepository;
using DaradsHubAPI.Core.Model;
using DaradsHubAPI.Core.Model.Request;
using DaradsHubAPI.Core.Services.Interface;
using DaradsHubAPI.Domain.Entities;
using DaradsHubAPI.Shared.Customs;
using Microsoft.EntityFrameworkCore;
using static DaradsHubAPI.Domain.Enums.Enum;

namespace DaradsHubAPI.Core.Services.Concrete;
public class ChatService(IUnitOfWork _unitOfWork) : IChatService
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

    public async Task<ApiResponse<IEnumerable<ChatMessageResponse>>> GetChatMessages(MessageListRequest request)
    {
        var messages = _unitOfWork.Notifications.GetChatMessages(request.ConversationId);

        var totalRecordsCount = messages.Count();
        var iMessages = await messages.Skip((request!.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToListAsync();
        return new ApiResponse<IEnumerable<ChatMessageResponse>> { Data = iMessages, Message = "Messages fetched successfully.", Status = true, StatusCode = StatusEnum.Success, Pages = request.PageSize, TotalRecord = totalRecordsCount, CurrentPageCount = request.PageNumber };
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
