using System.ComponentModel.DataAnnotations;

namespace DaradsHubAPI.Domain.Entities;
#nullable disable
public class MessageAudiences
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; }//All Customers
    public string Code { get; set; }//all
    public int TemplateId { get; set; }
}

public class MessageEmailTemplates
{
    [Key]
    public int Id { get; set; }
    public string TemplateName { get; set; }
    public string TemplateId { get; set; }
}
public class MessagesSentLogs
{
    [Key]
    public int Id { get; set; }
    public DateTime CreateDate { get; set; }
    public string MessageType { get; set; }
    public int TotalSent { get; set; }
    public string MsgId { get; set; }
}
public class MessagesSent
{
    [Key]
    public int Id { get; set; }
    public string Subject { get; set; }
    public string Message { get; set; }
    public string Audience { get; set; }
    public string AudienceCode
    { get; set; }
    public int TemplateId { get; set; }
    public string Status { get; set; }
    public DateTime SentDate { get; set; }
    public string MsgId { get; set; }
}

public class MessageThread
{
    [Key]
    public int Id { get; set; }
    [MaxLength(75)]
    public string CustomerId { get; set; }
    public int VendorId { get; set; }
    [MaxLength(1000)]
    public string Content { get; set; }
    public bool IsVendorMessage { get; set; }
    public DateTime DateMessageSent { get; set; }
}

public record MessageRequest(string Content);
public record UpdateMessageRequest(int Id, string Content);
public record MessageResponse
{
    public int Id { get; set; }
    public string CustomerId { get; set; }
    public int VendorId { get; set; }
    public string Content { get; set; }
    public string SentBy { get; set; }
    public DateTime DateMessageSent { get; set; }
};

public record VendorMessageRequest
{
    public string CustomerId { get; set; }
    public int VendorId { get; set; }
    public string Content { get; set; }
};
