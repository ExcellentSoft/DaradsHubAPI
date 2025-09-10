using DaradsHubAPI.Core.Model;
using DaradsHubAPI.Core.Model.Request;

namespace DaradsHubAPI.Core.Services.Interface;
public interface INotificationService
{
    Task<ApiResponse<IEnumerable<NotificationResponse>>> GetAllNotificationsAsync(NotificationListRequest request);
    Task<ApiResponse<IEnumerable<NotificationRequest>>> GetNotifications(string email);
    Task<ApiResponse> MarkAllNotificationAsRead(string email);
    Task<ApiResponse> MarkNotificationAsRead(long id);
}
