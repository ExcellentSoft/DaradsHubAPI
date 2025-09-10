using System.ComponentModel.DataAnnotations;

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
    }
}
