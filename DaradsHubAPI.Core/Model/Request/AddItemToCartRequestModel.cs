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

    public record AgentCustomerRequest : ListRequest
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }

    public record CustomerRequestsRequest : ListRequest
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public RequestStatus? Status { get; set; }
    }

    public record ProductOrderListRequest : ListRequest
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public OrderStatus? Status { get; set; }
        public long ProductId { get; set; }
        public bool IsDigital { get; set; }
    }

    public record AgentProductOrderListRequest : ListRequest
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public OrderStatus? Status { get; set; }
        public long AgentId { get; set; }
    }


    public record AgentReviewRequest : ListRequest
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }

    public record AgentOrderListResponse
    {
        public long OrderId { get; set; }
        public string? Code { get; set; }
        public CustomerData? CustomerDetails { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalPrice { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public string? ProductType { get; set; }

    }

    public record AgentCustomerMetricsResponse
    {
        public int TotalCustomer { get; set; }
        public int TotalActiveChat { get; set; }
        public int TotalPendingReplies { get; set; }
        public int TotalNewCustomer { get; set; }
    }

    public record AgentCustomerOrderResponse
    {
        public long ConversationId { get; set; }
        public LastCustomerMessage? LastMessage { get; set; }
        public CustomerOrderDetail? CustomerOrderDetail { get; set; }
    }

    public record LastCustomerMessage
    {
        public string? Content { get; set; }
        public string? LastInteractions { get; set; }
        public CustomerDetail? Customer { get; set; }
        public DateTime SentAt { get; internal set; }
        public int CustomerId { get; internal set; }
    }
    public record CustomerDetail
    {
        public string? FullName { get; set; }
        public string? Photo { get; set; }
        public bool? isOnline { get; internal set; }
    }
    public record CustomerOrderDetail
    {
        public long OrderId { get; set; }
        public string? OrderCode { get; set; }
        public int OrderCount { get; internal set; }
    }

    public record CustomerData
    {
        public string? Name { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
    }
    public record CustomerRequestResponse
    {
        public long RequestId { get; set; }
        public int Quantity { get; set; }
        public string Reference { get; set; } = default!;
        public string ProductService { get; set; } = default!;
        public string? ProductServiceImageUrl { get; set; }
        public DateTime PreferredDate { get; set; }
        public string? Location { get; set; }
        public bool IsUrgent { get; set; }
        public RequestStatus Status { get; set; }
        public DateTime DateCreated { get; set; }
        public RequestedUser? Customer { get; set; }

    }
    public record SingleCustomerRequestResponse
    {
        public long RequestId { get; set; }
        public int Quantity { get; set; }
        public string Reference { get; set; } = default!;
        public string ProductService { get; set; } = default!;
        public IEnumerable<string> ProductServiceImageUrls { get; set; } = default!;
        public DateTime PreferredDate { get; set; }
        public string? Location { get; set; }
        public string? Category { get; set; }
        public string ProductType { get; set; } = default!;
        public bool IsUrgent { get; set; }
        public RequestStatus Status { get; set; }
        public DateTime DateCreated { get; set; }
        public RequestedUser? Customer { get; set; }
        public decimal Budget { get; internal set; }
    }
    public record RequestedUser
    {
        public string? FullName { get; set; }
        public string? Photo { get; set; }
    }

    public record SingleOrderResponse
    {
        public long OrderId { get; set; }
        public string? Code { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalPrice { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public IEnumerable<OrderProductRecord> ProductDetails { get; set; } = default!;
        public IEnumerable<OrderActivitiesRecord> OrderActivitiesRecords { get; set; } = default!;
        public string? DeliveryMethod { get; set; }
        public CustomerOrderRecord? CustomerOrderRecord { get; set; }
    }

    public record CustomerOrderRecord
    {
        public string? Address { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? City { get; set; }
    }

    public record OrderActivitiesRecord
    {
        public string? Description { get; set; }
        public DateTime DateCreated { get; set; }
        public OrderStatus Status { get; internal set; }
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
    public record ChangeRequestStatus
    {
        public RequestStatus Status { get; set; }
        public long RequestId { get; set; }
    }
}
