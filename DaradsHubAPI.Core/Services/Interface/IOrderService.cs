using DaradsHubAPI.Core.Model;
using DaradsHubAPI.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaradsHubAPI.Core.Services.Interface;
public interface IOrderService
{
    Task<ApiResponse<IEnumerable<OrderListResponse>>> GetOrders(string email, OrderListRequest request);
}
