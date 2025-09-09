using System.ComponentModel.DataAnnotations;

namespace DaradsHubAPI.Core.Model.Request
{
    public class AddItemToCartRequestModel
    {
        public long ProductId { get; set; }
        public int Quantity { get; set; }
    }

    public class ShippingAddressRequest
    {
        public string Address { get; set; } = default!;
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Email { get; set; }
        public string PhoneNumber { get; set; } = default!;
    }

    public class ShippingAddressResponse
    {
        public long Id { get; set; }
        public string Address { get; set; } = default!;
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Email { get; set; }
        public string PhoneNumber { get; set; } = default!;
    }
}
