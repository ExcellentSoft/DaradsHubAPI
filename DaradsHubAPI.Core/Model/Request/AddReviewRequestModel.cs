namespace DaradsHubAPI.Core.Model.Request
{
    public class AddReviewRequestModel
    {

        public int productId { get; set; }
        public string Content { get; set; } = default!;
        public int Rating { get; set; }
    }
}
