using static DaradsHubAPI.Domain.Enums.Enum;

namespace DaradsHubAPI.Core.Model.Request
{
    public class OrderSmsRequestDto
    {
        public string country { get; set; }
        public int service { get; set; }
        public int? pool { get; set; }
        public int max_price { get; set; }
        public int pricing_option { get; set; }
        public int quantity { get; set; }
        public string areacode { get; set; }
        public int exclude { get; set; }
        public int create_token { get; set; } = 1;
        public string Key { get; set; }
    }
    public class OrderSmsNumberParam
    {
        public string UserId { get; set; }
        public string Country { get; set; }
        public int CountryId { get; set; }
        public int ServiceId { get; set; }
        public decimal ServicePrice { get; set; }
        public int? pool { get; set; }
        public int Max_price { get; set; }
        public int Quantity { get; set; }
        public string key { get; set; }

    }

    public record OrderHistoryRequest
    {
        public int PageSize { get; set; } = 10;
        public int PageNumber { get; set; } = 1;
        //public string? SearchText { get; set; }
        public string? TrendFilter { get; set; }
    }

    public record OrderHistoryResponse
    {
        public DateTime OrderDate { get; set; }
        public IEnumerable<OrderDetails> OrderDetails { get; set; } = default!;

    }

    public record OrderResponse
    {
        public string Code { get; set; } = default!;
        public string? UserEmail { get; set; }
        public string? PhoneNumber { get; set; }
        public string Status { get; set; } = default!;
        public int TotalProducts { get; set; }
        public DateTime Date { get; set; }
    }

    public record RejectedOrderResponse : OrderResponse
    {
        public string? Reason { get; set; }
    }

    public record RejectedOrderRequest
    {
        public string Reason { get; set; } = default!;
        public string OrderCode { get; set; } = default!;
    }

    public record OrderDetails
    {
        public string OrderCode { get; set; } = default!;
        public IEnumerable<string> ProductNames { get; set; } = default!;
        public IEnumerable<string> ProductImage { get; set; } = default!;
        public IEnumerable<string> ProductDescription { get; set; } = default!;
        public OrderStatus? Status { get; set; }
        public string? StatusText { get; set; }
        public string? Reason { get; set; }
    }

    public record OrderTrackingResponse
    {
        public string? Description { get; set; }
        public OrderStatus? Status { get; set; }
        public string? StatusText { get; set; }
        public DateTime DateCreated { get; set; }
    }

    public record OrderDetailResponse
    {
        public string OrderCode { get; set; } = default!;
        public OrderStatus? Status { get; set; }
        public string? StatusText { get; set; }
        public DateTime DateOrdered { get; set; }
        public IEnumerable<OrderProductDetails> OrderProductDetails { get; set; } = default!;

    }

    public record OrderProductDetails
    {

        public string? ProductNames { get; set; }
        public string? ProductImage { get; set; }
        public string? ProductDescription { get; set; }
    }

    public class OrderSummary
    {
        public int TotalOrder { get; set; }
        public int TotalReturnedOrder { get; set; }
        public int TotalDailyOrder { get; set; }
        public int TotalDelivered { get; set; }
        public int TotalAccepted { get; set; }
        public int TotalRejected { get; set; }
        public int TotalConfirmDelivered { get; set; }
        public int TotalShipping { get; set; }
        public int TotalProducts { get; set; }
        public int TotalProductsQuantities { get; set; }
        public decimal TotalSalesAmount { get; set; }
        public decimal TotalPendingSalesAmount { get; set; }
    }

    public record OrderRequest
    {

        public string orderCode { get; set; } = default!;
        public OrderStatus OrderStatus { get; set; }
    }
}