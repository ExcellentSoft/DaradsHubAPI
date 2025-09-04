using DaradsHubAPI.Core.IRepository;
using DaradsHubAPI.Core.Model;
using DaradsHubAPI.Core.Services.Interface;
using DaradsHubAPI.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using static DaradsHubAPI.Domain.Enums.Enum;

namespace DaradsHubAPI.Core.Services.Concrete;
public class OrderService(IUnitOfWork _unitOfWork) : IOrderService
{

    public async Task<ApiResponse<IEnumerable<OrderListResponse>>> GetOrders(string email, OrderListRequest request)
    {
        var query = _unitOfWork.Orders.GetOrders(email, request);

        var totalOrders = query.Count();
        var paginatedOrders = await Task.FromResult(query.Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize).ToList());

        return new ApiResponse<IEnumerable<OrderListResponse>> { Message = "Orders fetched successfully.", Status = true, Data = paginatedOrders, StatusCode = StatusEnum.Success, TotalRecord = totalOrders, Pages = request.PageSize, CurrentPageCount = request.PageNumber };
    }
}
