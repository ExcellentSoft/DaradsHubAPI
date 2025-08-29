namespace DaradsHubAPI.Core.Model.Response
{
    public class CartResponse
    {
        public int userId { get; set; }
        public string? sessionId { get; set; }
        public string ProductName { get; set; }
        public bool SendNotification { get; set; }
        public long? ProductId { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal? SubTotal { get; set; }
        public int Id { get; set; }
        public string ImageUrl { get; set; }
        public string Description { get; set; }
    }
}
