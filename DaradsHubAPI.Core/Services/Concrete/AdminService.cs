using DaradsHubAPI.Core.IRepository;
using DaradsHubAPI.Core.Model;
using DaradsHubAPI.Core.Model.Request;
using DaradsHubAPI.Core.Services.Interface;
using static DaradsHubAPI.Domain.Enums.Enum;

namespace DaradsHubAPI.Core.Services.Concrete;
public class AdminService(IUnitOfWork _unitOfWork) : IAdminService
{
    public async Task<ApiResponse<DailySalesOverviewResponse>> DailySalesOverview()
    {
        var responses = await _unitOfWork.Orders.DailySalesOverview();
        return new ApiResponse<DailySalesOverviewResponse> { Message = "Successful", Status = true, Data = responses ?? new DailySalesOverviewResponse(), StatusCode = StatusEnum.Success };
    }

    public async Task<ApiResponse<AdminDashboardMetricResponse>> GetDashboardMetrics()
    {
        var responses = await _unitOfWork.Orders.AdminDashboardMetrics();
        return new ApiResponse<AdminDashboardMetricResponse> { Message = "Successful", Status = true, Data = responses, StatusCode = StatusEnum.Success };
    }

    public async Task<ApiResponse<IEnumerable<TopPerformingAgentResponse>>> TopPerformingAgents()
    {
        var responses = await _unitOfWork.Orders.TopPerformingAgents2();
        return new ApiResponse<IEnumerable<TopPerformingAgentResponse>> { Message = "Successful", Status = true, Data = responses, StatusCode = StatusEnum.Success };
    }

    public async Task<ApiResponse<IEnumerable<LastFourCustomerRequest>>> PendingCustomerRequests()
    {
        var responses = await _unitOfWork.Orders.GetLastCustomerRequests();
        return new ApiResponse<IEnumerable<LastFourCustomerRequest>> { Message = "Successful", Status = true, Data = responses, StatusCode = StatusEnum.Success };
    }
}