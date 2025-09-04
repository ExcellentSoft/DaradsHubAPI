using DaradsHubAPI.Domain.Entities;

namespace DaradsHubAPI.Core.IRepository;

public interface IOrderRepository : IGenericRepository<HubOrder>
{
    IQueryable<OrderListResponse> GetOrders(string email, OrderListRequest request);
}