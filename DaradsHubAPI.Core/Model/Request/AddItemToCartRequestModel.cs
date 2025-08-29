namespace DaradsHubAPI.Core.Model.Request
{
    public class AddItemToCartRequestModel
    {
        public long ProductId { get; set; }
        public int UserId { get; set; }
        public int Quantity { get; set; }

    }
}
