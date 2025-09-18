using DaradsHubAPI.Core.Model;
using DaradsHubAPI.Core.Model.Request;
using DaradsHubAPI.Core.Model.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaradsHubAPI.Core.Services.Interface;
public interface IAgentDashboardService
{
    Task<ApiResponse<CatalogueInsightResponse>> GetCatalogueInsight(int agentId);
    Task<ApiResponse<DashboardMetricResponse>> GetDashboardMetrics(int agentId, string userEmail);
    Task<ApiResponse<IEnumerable<AgentReview>>> GetLatestReview(int agentId);
}
