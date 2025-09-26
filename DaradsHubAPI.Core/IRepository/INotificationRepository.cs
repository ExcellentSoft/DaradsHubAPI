using DaradsHubAPI.Core.Model.Request;
using DaradsHubAPI.Domain.Entities;

namespace DaradsHubAPI.Core.IRepository;
public interface INotificationRepository : IGenericRepository<HubNotification>
{
    Task DeleteNotification(long Id);
    IQueryable<ViewChatMessagesResponse> GetAgentChatMessages(int agentId);
    Task<List<NotificationRequest>> GetAllNotificationsAsync(string email);
    IQueryable<NotificationResponse> GetAllNotificationsAsync(NotificationListRequest request);
    IQueryable<ChatMessageResponse> GetChatMessages(long conversationId);
    IQueryable<ViewChatMessagesResponse> GetCustomerChatMessages(int customerId);
    Task<HubChatConversation> GetOrCreateConversation(CreateConversationRequest request);
    Task MarkAllMessageAsRead(long conversationId);
    Task MarkAllNotificationAsRead(string email);
    Task MarkNotificationAsRead(long id);
    Task SaveChatMessage(HubChatMessage entity);
    Task SaveNotification(HubNotification entity);
}
