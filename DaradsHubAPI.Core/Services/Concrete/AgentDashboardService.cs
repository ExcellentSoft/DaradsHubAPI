using DaradsHubAPI.Core.IRepository;
using DaradsHubAPI.Core.Model;
using DaradsHubAPI.Core.Model.Request;
using DaradsHubAPI.Core.Model.Response;
using DaradsHubAPI.Core.Services.Interface;
using static DaradsHubAPI.Domain.Enums.Enum;

namespace DaradsHubAPI.Core.Services.Concrete;
public class AgentDashboardService(IUnitOfWork _unitOfWork) : IAgentDashboardService
{
    public async Task<ApiResponse<DashboardMetricResponse>> GetDashboardMetrics(int agentId, string userEmail)
    {
        var responses = await _unitOfWork.Products.GetDashboardMetrics(agentId, userEmail);
        return new ApiResponse<DashboardMetricResponse> { Message = "Successful", Status = true, Data = responses, StatusCode = StatusEnum.Success };
    }

    public async Task<ApiResponse<CatalogueInsightResponse>> GetCatalogueInsight(int agentId)
    {
        var responses = await _unitOfWork.Orders.GetCatalogueInsight(agentId);
        return new ApiResponse<CatalogueInsightResponse> { Message = "Successful", Status = true, Data = responses, StatusCode = StatusEnum.Success };
    }

    public async Task<ApiResponse<IEnumerable<AgentReview>>> GetLatestReview(int agentId)
    {
        var review = await _unitOfWork.Products.GetLatestReview(agentId);

        return new ApiResponse<IEnumerable<AgentReview>> { Data = review, Message = "Successful", Status = true, StatusCode = StatusEnum.Success };
    }
}