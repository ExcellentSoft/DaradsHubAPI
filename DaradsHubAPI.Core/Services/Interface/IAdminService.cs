using DaradsHubAPI.Core.Model;
using DaradsHubAPI.Core.Model.Request;

namespace DaradsHubAPI.Core.Services.Interface;
public interface IAdminService
{
    Task<ApiResponse<AdminDashboardMetricResponse>> GetDashboardMetrics();
}
