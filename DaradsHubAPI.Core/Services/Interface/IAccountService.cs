using DaradsHubAPI.Core.Model;
using DaradsHubAPI.Core.Model.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaradsHubAPI.Core.Services.Interface;
public interface IAccountService
{
    Task<ApiResponse> ChangePassword(ChangePasswordRequest request, string email);
    Task<ApiResponse<AgentProfileResponse>> GetAgentProfile(string email);
    Task<ApiResponse<CustomerProfileResponse>> GetCustomerProfile(string email);
    Task<ApiResponse> UpdateAgentProfile(AgentProfileRequest request, string email);
    Task<ApiResponse> UpdateProfile(CustomerProfileRequest request, string email);
}
