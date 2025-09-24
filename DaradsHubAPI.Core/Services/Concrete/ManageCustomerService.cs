using DaradsHubAPI.Core.IRepository;
using DaradsHubAPI.Core.Model;
using DaradsHubAPI.Core.Model.Request;
using DaradsHubAPI.Core.Model.Response;
using DaradsHubAPI.Core.Services.Interface;
using static DaradsHubAPI.Domain.Enums.Enum;

namespace DaradsHubAPI.Core.Services.Concrete;
public class ManageCustomerService(IUnitOfWork _unitOfWork) : IManageCustomerService
{
    public async Task<ApiResponse<IEnumerable<CustomerRequestResponse>>> GetCustomerRequestsForAdmin(CustomerRequestsRequest request)
    {
        var customerRequests = await _unitOfWork.Products.GetCustomerRequestsForAdmin(request);

        return new ApiResponse<IEnumerable<CustomerRequestResponse>> { Data = customerRequests, Message = "Successful", Status = true, StatusCode = StatusEnum.Success, TotalRecord = customerRequests.Count, Pages = request.PageSize, CurrentPageCount = request.PageNumber };
    }

    public async Task<ApiResponse<CustomerRequestMetricResponse>> GetCustomerRequestMetricsForAdmin()
    {
        var responses = await _unitOfWork.Products.GetCustomerRequestMetricsForAdmin();
        return new ApiResponse<CustomerRequestMetricResponse>
        {
            Message = "Successful",
            Status = true,
            Data = responses,
            StatusCode = StatusEnum.Success

        };
    }
}
