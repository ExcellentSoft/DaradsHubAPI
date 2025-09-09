using DaradsHubAPI.Core.Model.Request;
using DaradsHubAPI.Core.Model.Response;
using DaradsHubAPI.Domain.Entities;

namespace DaradsHubAPI.Core.IRepository;

public interface IOrderRepository : IGenericRepository<HubOrder>
{
    Task AddCart(shopCat model);
    Task AddHubOrderTracking(HubOrderTracking model);
    void AddOrderItem(HubOrderItem model);
    Task AddShippingAddress(ShippingAddress model);
    Task DeleteCart(int userId, long productId);
    Task DeleteShippingAddress(int userId, long addressId);
    Task<shopCat?> GetCart(int userId, long productId);
    IQueryable<CartResponse> GetCartsListByUserId(int userId);
    IQueryable<OrderListResponse> GetOrders(string email, OrderListRequest request);
    IQueryable<ShippingAddress> GetShippingAddresses(int userId);
    Task UpdateCart(shopCat model);
}