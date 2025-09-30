using DaradsHubAPI.Core.Services.Interface;
using DaradsHubAPI.Shared.Customs;
using Microsoft.AspNetCore.SignalR;

namespace DaradsHubAPI.WebAPI.ChatHelper;

public class ChatHub(IChatService _chatService) : Hub
{
    public async Task SendMessage(string conversationId, string senderId, string message)
    {
        var today = GetLocalDateTime.CurrentDateTime().ToString();
        await _chatService.AddMessageAsync(long.Parse(conversationId), int.Parse(senderId), message);

        var chatMessage = new
        {
            conversationId,
            content = message,
            sentAt = today,
            sender = new
            {
                userId = senderId,
                fullName = "Other Sender", // Or fetch from DB
                isAgent = false,      // Or set dynamically
                photo = ""
            },
            isRead = false,
            messageId = Guid.NewGuid()
        };

        await Clients.Group(conversationId)
            .SendAsync("ReceiveMessage", chatMessage);
    }

    public async Task JoinConversation(int conversationId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, conversationId.ToString());
    }

}