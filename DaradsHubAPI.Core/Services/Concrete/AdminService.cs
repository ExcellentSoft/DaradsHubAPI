using DaradsHubAPI.Core.IRepository;
using DaradsHubAPI.Core.Model;
using DaradsHubAPI.Core.Model.Request;
using DaradsHubAPI.Core.Model.Response;
using DaradsHubAPI.Core.Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DaradsHubAPI.Domain.Enums.Enum;

namespace DaradsHubAPI.Core.Services.Concrete;
public class AdminService(IUnitOfWork _unitOfWork) : IAdminService
{
    public async Task<ApiResponse<AdminDashboardMetricResponse>> DailySalesOverview()
    {
        var responses = await _unitOfWork.Orders.DailySalesOverview();
        return new ApiResponse<AdminDashboardMetricResponse> { Message = "Successful", Status = true, /*Data = responses,*/ StatusCode = StatusEnum.Success };
    }
    public async Task<ApiResponse<AdminDashboardMetricResponse>> GetDashboardMetrics()
    {
        // var responses = await _unitOfWork.Products.GetDashboardMetrics(agentId, userEmail);
        return new ApiResponse<AdminDashboardMetricResponse> { Message = "Successful", Status = true, /*Data = responses,*/ StatusCode = StatusEnum.Success };
    }
}