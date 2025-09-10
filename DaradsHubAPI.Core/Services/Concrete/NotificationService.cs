using Microsoft.EntityFrameworkCore;
using DaradsHubAPI.Core.Services.Interface;
using DaradsHubAPI.Core.IRepository;
using DaradsHubAPI.Core.Model;
using DaradsHubAPI.Core.Model.Request;
using static DaradsHubAPI.Domain.Enums.Enum;

namespace DaradsHubAPI.Core.Services.Concrete;
public class NotificationService(IUnitOfWork _unitOfWork) : INotificationService
{
    public async Task<ApiResponse<IEnumerable<NotificationRequest>>> GetNotifications(string email)
    {
        var notifications = await _unitOfWork.Notifications.GetAllNotificationsAsync(email);

        return new ApiResponse<IEnumerable<NotificationRequest>>
        {
            Data = notifications,
            Message = "Notifications fetched successfully.",
            Status = true,
            StatusCode = StatusEnum.Success
        };
    }

    public async Task<ApiResponse> MarkAllNotificationAsRead(string email)
    {
        await _unitOfWork.Notifications.MarkAllNotificationAsRead(email);
        return new ApiResponse("Success.", StatusEnum.Success, true);
    }

    public async Task<ApiResponse> MarkNotificationAsRead(long id)
    {
        await _unitOfWork.Notifications.MarkNotificationAsRead(id);
        return new ApiResponse("Success.", StatusEnum.Success, true);
    }

    public async Task<ApiResponse<IEnumerable<NotificationResponse>>> GetAllNotificationsAsync(NotificationListRequest request)
    {
        var notification = _unitOfWork.Notifications.GetAllNotificationsAsync(request);

        var totalRecordsCount = notification.Count();
        var iNotifications = await notification.Skip((request!.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToListAsync();
        return new ApiResponse<IEnumerable<NotificationResponse>> { Data = iNotifications, Message = "Notification fetched successfully.", Status = true, StatusCode = StatusEnum.Success, Pages = request.PageSize, TotalRecord = totalRecordsCount, CurrentPageCount = request.PageNumber };
    }
}