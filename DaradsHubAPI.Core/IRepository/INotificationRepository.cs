using DaradsHubAPI.Core.Model.Request;
using DaradsHubAPI.Domain.Entities;

namespace DaradsHubAPI.Core.IRepository;
public interface INotificationRepository : IGenericRepository<HubNotification>
{
    Task DeleteNotification(long Id);
    IQueryable<ViewChatMessagesResponse> GetAdminChatMessages(int adminId);
    IQueryable<ViewChatMessagesResponse> GetAgentChatMessages(int agentId);
    Task<List<NotificationRequest>> GetAllNotificationsAsync(string email);
    IQueryable<NotificationResponse> GetAllNotificationsAsync(NotificationListRequest request);
    IQueryable<ChatMessageResponse> GetChatMessages(long conversationId);
    IQueryable<ViewChatMessagesResponse> GetChatMessages();
    IQueryable<ViewChatMessagesResponse> GetCustomerChatMessages(int customerId);
    Task<HubChatConversation> GetOrCreateAdminConversationWithAgent(CreateAdminConversationWithAgentRequest request);
    Task<HubChatConversation> GetOrCreateAdminConversationWithCustomer(CreateAdminConversationWithCustomerRequest request);
    Task<HubChatConversation> GetOrCreateConversation(CreateConversationRequest request);
    IQueryable<AgentDatum> GetUnreadChatMessages();
    Task<(bool status, string message)> JoinConversation(JoinConversationRequest request);
    Task MarkAllMessageAsRead(long conversationId);
    Task MarkAllNotificationAsRead(string email);
    Task MarkNotificationAsRead(long id);
    Task ReportAgent(ReportAgent report);
    Task SaveChatMessage(HubChatMessage entity);
    Task SaveNotification(HubNotification entity);
}
