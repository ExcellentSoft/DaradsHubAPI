using DaradsHubAPI.Core.Services.Interface;
using DaradsHubAPI.Shared.Customs;
using Microsoft.AspNetCore.SignalR;

namespace DaradsHubAPI.WebAPI.ChatHelper;

public class ChatHub(IChatService _chatService) : Hub
{
    public async Task SendMessageToConversation(string conversationId, string senderId, string message)
    {
        var today = GetLocalDateTime.CurrentDateTime();
        await _chatService.AddMessageAsync(long.Parse(conversationId), int.Parse(senderId), message);

        await Clients.Group(conversationId)
            .SendAsync("ReceiveMessage", senderId, message, today);
    }
}