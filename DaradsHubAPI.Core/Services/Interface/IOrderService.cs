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
    Task<ApiResponse<string>> CheckOut(CheckoutRequest request, string email, int userId);
    Task<ApiResponse<IEnumerable<CartResponse>>> GetCart(int userId);
    Task<ApiResponse<IEnumerable<OrderListResponse>>> GetOrders(string email, OrderListRequest request);
    Task<ApiResponse<IEnumerable<ShippingAddressResponse>>> GetShippingAddress(int userId);
    Task<ApiResponse> RemoveItemsFromCart(AddItemToCartRequestModel addToCartRequestModel, int userId);
    Task<ApiResponse> RemoveShippingAddress(int userId, long addressId);
}
