using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaradsHubAPI.Core.Model.Request;
public record OnlineAgentsResponse
{
    public string? FullName { get; set; }
    public string? Photo { get; set; }
    public int userId { get; set; }
}

public record CreateConversationRequest
{
    public int CustomerId { get; set; }
    public int AgentId { get; set; }
}
public record CreateAdminConversationWithAgentRequest
{
    public int AdminId { get; set; }
    public int AgentId { get; set; }
}
public record CreateAdminConversationWithCustomerRequest
{
    public int AdminId { get; set; }
    public int CustomerId { get; set; }
}
public record JoinConversationRequest
{
    public long ConversationId { get; set; }
    public int AdminId { get; set; }
}
