using DaradsHubAPI.Core.Model.Request;
using DaradsHubAPI.Domain.Entities;

namespace DaradsHubAPI.Core.IRepository;
public interface INotificationRepository : IGenericRepository<HubNotification>
{
    Task DeleteNotification(long Id);
    Task<List<NotificationRequest>> GetAllNotificationsAsync(string email);
    IQueryable<NotificationResponse> GetAllNotificationsAsync(NotificationListRequest request);
    Task MarkAllNotificationAsRead(string email);
    Task MarkNotificationAsRead(long id);
    Task SaveNotification(HubNotification entity);
}
