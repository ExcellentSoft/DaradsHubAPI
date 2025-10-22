using DaradsHubAPI.Core.Model;
using DaradsHubAPI.Core.Model.Request;
using DaradsHubAPI.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaradsHubAPI.Core.Services.Interface;
public interface IChatService
{
    Task AddMessageAsync(long conversationId, int senderId, string message);
    Task<ApiResponse> ChangeAgentOnlineStatus(bool isOnline, int agentId);
    Task<ApiResponse<HubChatConversation>> CreateConversation(CreateConversationRequest request);
    Task<ApiResponse<IEnumerable<ViewChatMessagesResponse>>> GetAdminChatMessages(int adminId);
    Task<ApiResponse<IEnumerable<ViewChatMessagesResponse>>> GetAgentChatMessages(int agentId);
    Task<ApiResponse<IEnumerable<ChatMessageResponse>>> GetChatMessages(MessageListRequest request);
    Task<ApiResponse<IEnumerable<ViewChatMessagesResponse>>> GetChatMessages();
    Task<ApiResponse<IEnumerable<ViewChatMessagesResponse>>> GetCustomerChatMessages(int customerId);
    Task<ApiResponse<IEnumerable<OnlineAgentsResponse>>> GetOnlineAgents(string? searchText);
    Task<ApiResponse<HubChatConversation>> GetOrCreateAdminConversationWithAgent(CreateAdminConversationWithAgentRequest request);
    Task<ApiResponse<HubChatConversation>> GetOrCreateAdminConversationWithCustomer(CreateAdminConversationWithCustomerRequest request);
    Task GetUnreadChatMessages();
    Task<ApiResponse> JoinConversation(JoinConversationRequest request);
    Task<ApiResponse> MarkAllMessageAsRead(long conversationId);
    Task<ApiResponse> ReportAgent(ReportAgentRequest request);
}
