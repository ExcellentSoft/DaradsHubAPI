using DaradsHubAPI.Core.Model;
using DaradsHubAPI.Core.Model.Request;
using DaradsHubAPI.Core.Model.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaradsHubAPI.Core.Services.Interface;
public interface IManageCustomerService
{
    Task<ApiResponse<CustomerRequestMetricResponse>> GetCustomerRequestMetricsForAdmin();
    Task<ApiResponse<IEnumerable<CustomerRequestResponse>>> GetCustomerRequestsForAdmin(CustomerRequestsRequest request);
}
