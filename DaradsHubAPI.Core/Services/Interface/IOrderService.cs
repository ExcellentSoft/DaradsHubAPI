using DaradsHubAPI.Core.Model;
using DaradsHubAPI.Core.Model.Request;
using DaradsHubAPI.Core.Model.Response;
using DaradsHubAPI.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaradsHubAPI.Core.Services.Interface;
public interface IOrderService
{
    Task<ApiResponse> AddItemsToCart(AddItemToCartRequestModel request, int userId);
    Task<ApiResponse> AddShippingAddress(ShippingAddressRequest request, int userId);
    Task<ApiResponse> ChangeOrderStatus(ChangeStatusRequest request);
    Task<ApiResponse<string>> CheckOut(CheckoutRequest request, string email, int userId);
    Task<ApiResponse<DigitalCheckoutResponse>> CheckOutDigital(CheckoutDigitalRequest request, string email);
    Task<ApiResponse<IEnumerable<CartResponse>>> GetCart(int userId);
    Task<ApiResponse<SingleOrderResponse>> GetOrder(string orderCode);
    Task<ApiResponse<IEnumerable<OrderListResponse>>> GetOrders(string email, OrderListRequest request);
    Task<ApiResponse<List<AgentOrderListResponse>>> GetOrders(AgentOrderListRequest request, int agentId);
    Task<ApiResponse<IEnumerable<ShippingAddressResponse>>> GetShippingAddress(int userId);
    Task<ApiResponse<AgentOrderMetricResponse>> OrderMetrics(int agentId);
    Task<ApiResponse> RemoveItemsFromCart(AddItemToCartRequestModel addToCartRequestModel, int userId);
    Task<ApiResponse> RemoveShippingAddress(int userId, long addressId);
}
