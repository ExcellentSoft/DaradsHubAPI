using static DaradsHubAPI.Domain.Enums.Enum;

namespace DaradsHubAPI.Core.Model.Request
{
    public class CheckOutRequestModel
    {
        public int Id { get; set; }
        public string? ProductName { get; set; }
        public int ProductId { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal SubTotal { get; set; }
    }

    public record CheckoutRequest
    {
        public decimal Price { get; set; }
        public int ShippingAddressId { get; set; }
        public DeliveryMethodType DeliveryMethodType { get; set; }
        public IEnumerable<ProductDetails> ProductDetails { get; set; } = default!;
    }

    public record ProductDetails
    {
        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        public int Quantity { get; set; }
    }

}
