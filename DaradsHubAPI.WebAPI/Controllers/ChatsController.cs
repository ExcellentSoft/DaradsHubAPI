using DaradsHubAPI.Core.Model;
using DaradsHubAPI.Core.Model.Request;
using DaradsHubAPI.Core.Services.Interface;
using DaradsHubAPI.Domain.Entities;
using DaradsHubAPI.WebAPI.Extensions;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace DaradsHubAPI.WebAPI.Controllers;

public class ChatsController(IChatService _chatService) : ApiBaseController
{
    [HttpGet("online-agents")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<OnlineAgentsResponse>>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetOnlineAgents(string? searchText)
    {
        var response = await _chatService.GetOnlineAgents(searchText);
        return ResponseCode(response);
    }

    [HttpPost("create-conversation")]
    [ProducesResponseType(typeof(ApiResponse<HubChatConversation>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetOnlineAgents([FromBody] CreateConversationRequest request)
    {
        var response = await _chatService.CreateConversation(request);
        return ResponseCode(response);
    }

    [HttpPatch("change-agent-online-status")]
    [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> ChangeAgentOnlineStatus([FromQuery] bool isOnline)
    {
        var agentId = int.Parse(User.Identity?.GetUserId() ?? "");
        var response = await _chatService.ChangeAgentOnlineStatus(isOnline, agentId);
        return ResponseCode(response);
    }

    [HttpGet("messages")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<ChatMessageResponse>>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetChatMessages([FromQuery] MessageListRequest request)
    {
        var response = await _chatService.GetChatMessages(request);
        return ResponseCode(response);
    }

    [HttpGet("agent-chats")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<ViewChatMessagesResponse>>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetAgentChatMessages([FromQuery] int agentId)
    {
        var response = await _chatService.GetAgentChatMessages(agentId);
        return ResponseCode(response);
    }

    [HttpGet("customer-chats")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<ViewChatMessagesResponse>>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetCustomerChatMessages([FromQuery] int customerId)
    {
        var response = await _chatService.GetCustomerChatMessages(customerId);
        return ResponseCode(response);
    }

    [HttpPatch("mark-all-messages-as-read")]
    [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> MarkAllMessageAsRead([FromQuery] long conversationId)
    {
        var response = await _chatService.MarkAllMessageAsRead(conversationId);
        return ResponseCode(response);
    }
}