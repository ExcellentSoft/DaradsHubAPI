using DaradsHubAPI.Core.Model;
using DaradsHubAPI.Core.Model.Response;
using DaradsHubAPI.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaradsHubAPI.Core.Services.Interface;
public interface IAuthService
{
    Task<ApiResponse<string>> CreateAgent(CreateAgentRequest request);
    Task<ApiResponse<string>> CreateCustomer(CreateCustomerRequest request);
    Task<ApiResponse<string>> ForgetPassword(ForgetPasswordRequest request);
    Task<ApiResponse<CustomerLoginResponse>> LoginUser(LoginRequest request);
    Task<ApiResponse> ResendEmailVerificationCode(string userId);
    Task<ApiResponse> ResendResetPasswordCode(string email);
    Task<ApiResponse> ResetPassword(ResetPasswordRequest request);
    Task<ApiResponse<CustomerLoginResponse>> VerifyUserAccount(string code);
}
