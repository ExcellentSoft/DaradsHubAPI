using DaradsHubAPI.Core.Model;
using DaradsHubAPI.Core.Model.Request;
using DaradsHubAPI.Core.Services.Interface;
using DaradsHubAPI.WebAPI.Extensions;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace DaradsHubAPI.WebAPI.Controllers;

public class NotificationsController(INotificationService _notificationService) : ApiBaseController
{
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<NotificationRequest>>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetNotification()
    {
        var email = User.Identity?.GetUserEmail() ?? "";
        var response = await _notificationService.GetNotifications(email);
        return ResponseCode(response);
    }

    [HttpPatch("mark-all-notification-as-read")]
    [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> MarkAllNotificationAsRead()
    {
        var email = User.Identity?.GetUserEmail() ?? "";
        var response = await _notificationService.MarkAllNotificationAsRead(email);
        return ResponseCode(response);
    }

    [HttpPatch("{id:long}/mark-notification-as-read")]
    [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> MarkNotificationAsRead([FromRoute] long id)
    {
        var response = await _notificationService.MarkNotificationAsRead(id);
        return ResponseCode(response);
    }
}