using System.ComponentModel.DataAnnotations;
using static DaradsHubAPI.Domain.Enums.Enum;

namespace DaradsHubAPI.Core.Model.Request
{
    public class AddReviewRequestModel
    {
        public int productId { get; set; }
        public string Content { get; set; } = default!;
        public int Rating { get; set; }
    }
    public class AddAgentReviewRequest
    {
        public int AgentId { get; set; }
        public string Content { get; set; } = default!;
        public int Rating { get; set; }
    }

    public class AgentReviewResponse
    {
        public int TotalReviewCount { get; set; }
        public double RatingAverage { get; set; }
        public IEnumerable<AgentReview> Reviews { get; set; } = default!;

    }

    public class AgentReview
    {
        public string? ReviewBy { get; set; }
        public string? Content { get; set; }
        public int Rating { get; set; }
        public DateTime ReviewDate { get; set; }
        public string? ReviewerPhoto { get; set; }
    }

    public class ProductReviewResponse
    {
        public int TotalReviewCount { get; set; }
        public double RatingAverage { get; set; }
        public IEnumerable<ProductReview> Reviews { get; set; } = default!;

    }

    public class ProductReview
    {
        public string? ReviewBy { get; set; }
        public string? Content { get; set; }
        public int Rating { get; set; }
        public bool IsDigital { get; set; }
        public DateTime ReviewDate { get; set; }
        public string? ReviewerPhoto { get; set; }
    }

    public class NotificationRequest
    {
        public string Message { get; set; } = default!;
        public long Id { get; set; }
        public string? Title { get; set; }
        public DateTime NotificationDate { get; set; }
        public string Duration { get; internal set; }
    }
    public class NotificationResponse
    {
        public long Id { get; set; }
        public string Message { get; set; } = default!;
        public string? Title { get; set; }
        public string? Duration { get; set; }
        public NotificationType NotificationType { get; set; }
    }

    public record NotificationListRequest
    {
        public int PageSize { get; set; } = 10;
        public int PageNumber { get; set; } = 1;
        public NotificationType? NotificationType { get; set; }
    }

    public record MessageListRequest
    {
        public int PageSize { get; set; } = 10;
        public int PageNumber { get; set; } = 1;
        public long ConversationId { get; set; }
    }
    public record ChatMessageResponse
    {
        public long ConversationId { get; set; }
        public SenderDetails Sender { get; set; } = default!;
        public string Content { get; set; } = default!;
        public DateTime SentAt { get; set; }
        public bool IsRead { get; set; }
        public long MessageId { get; internal set; }
    }

    public record SenderDetails
    {
        public string? FullName { get; set; }
        public string? Photo { get; set; }
        public int userId { get; set; }
        public bool? IsAgent { get; internal set; }
    }
}
