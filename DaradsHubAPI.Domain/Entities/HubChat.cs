using System.ComponentModel.DataAnnotations;

namespace DaradsHubAPI.Domain.Entities;

public class HubChatConversation
{
    [Key]
    public long Id { get; set; }
    public int AgentId { get; set; }
    public int CustomerId { get; set; }
    public int AdminId { get; set; }
    public DateTime DateCreated { get; set; }
}

public class HubChatMessage
{
    [Key]
    public long Id { get; set; }
    public long ConversationId { get; set; }
    public int SenderId { get; set; }
    public string Content { get; set; } = default!;
    public DateTime SentAt { get; set; }
    public bool IsRead { get; set; }
}

//create conversation done
//send new message
//receive new message
//get  chat history
