using DaradsHubAPI.Domain.Entities;
using System.ComponentModel.DataAnnotations;
using static DaradsHubAPI.Domain.Enums.Enum;

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

    public class AgentOrderMetricResponse
    {
        public int TotalOrderCount { get; set; }
        public int RefundedOrderCount { get; set; }
        public int CanceledOrderCount { get; set; }
        public int CompletedOrderCount { get; set; }
        public int PendingOrderCount { get; internal set; }
        public int ProcessingOrderCount { get; internal set; }
    }
    public class CatalogueInsightResponse
    {
        public int TotalDigitalProductCount { get; set; }
        public BestSeller? BestSeller { get; set; }

    }
    public class BestSeller
    {
        public string Name { get; set; } = default!;
        public int TotalSales { get; set; }
    }
    public record AgentOrderListRequest : ListRequest
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public OrderStatus? Status { get; set; }
    }

    public record ProductOrderListRequest : ListRequest
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public OrderStatus? Status { get; set; }
        public long ProductId { get; set; }
        public bool IsDigital { get; set; }
    }

    public record AgentOrderListResponse
    {
        public long OrderId { get; set; }
        public string? Code { get; set; }
        public string? CustomerName { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalPrice { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public string? ProductType { get; set; }

    }
    public record SingleOrderResponse
    {
        public long OrderId { get; set; }
        public string? Code { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalPrice { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public IEnumerable<OrderProductRecord> ProductDetails { get; set; } = default!;
    }
    public record OrderProductRecord
    {
        public string Name { get; set; } = default!;
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
    public record ChangeStatusRequest
    {
        public OrderStatus Status { get; set; }
        public string OrderCode { get; set; } = default!;
    }
}
