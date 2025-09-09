namespace DaradsHubAPI.Core.Model.Response
{
    public class CartResponse
    {
        public int UserId { get; set; }
        public string ProductName { get; set; } = default!;
        public long? ProductId { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal? SubTotal { get; set; }
        public int Id { get; set; }
        public string? ImageUrl { get; set; }
        public string? Description { get; set; }
        public string? AgentName { get; set; }
    }
}