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
public interface IManageCustomerService
{
    Task<ApiResponse<CustomerMetricsResponse>> GetCustomerMetrics();
    Task<ApiResponse<ShortCustomerProfileResponse>> GetCustomerProfile(int customerId);
    Task<ApiResponse<CustomerRequestMetricResponse>> GetCustomerRequestMetricsForAdmin();
    Task<ApiResponse<IEnumerable<CustomerRequestResponse>>> GetCustomerRequestsForAdmin(CustomerRequestsRequest request);
    Task<ApiResponse<IEnumerable<CustomersListResponse>>> GetCustomers(CustomersListRequest request);
    Task<ApiResponse<IEnumerable<OrderListResponse>>> GetRecentOrders(string email, OrderListRequest request);
    Task<ApiResponse> UpdateCustomerStatus(CustomerStatusRequest request);
}
