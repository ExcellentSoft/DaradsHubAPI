using DaradsHubAPI.Core.IRepository;
using DaradsHubAPI.Core.Model;
using DaradsHubAPI.Core.Model.Response;
using DaradsHubAPI.Core.Services.Interface;
using DaradsHubAPI.Domain.Entities;
using DaradsHubAPI.Shared.Extentions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DaradsHubAPI.Domain.Enums.Enum;

namespace DaradsHubAPI.Core.Services.Concrete;
public class AuthService(IUnitOfWork _unitOfWork) : IAuthService
{
    public async Task<ApiResponse<string>> CreateCustomer(CreateCustomerRequest request)
    {
        var validateResult = ValidateCustomerRequest(request);
        if (!validateResult.Status.GetValueOrDefault())
            return new ApiResponse<string> { Status = validateResult.Status, Message = validateResult.Message, StatusCode = StatusEnum.Validation };

        request.Email = request.Email.Trim().ToLower();

        var customer = await _unitOfWork.Users.GetSingleWhereAsync(u => u.email == request.Email);

        if (await _unitOfWork.Users.AnyAsync(us => us.email == request.Email && us.phone == request.PhoneNumber))
        {
            return new ApiResponse<string> { Status = false, Message = $"Customer with {request.FullName} already exists. Please verify and try again.", StatusCode = StatusEnum.Validation };
        }

        if (_unitOfWork.Users.Any(us => us.email == request.Email))
        {
            return new ApiResponse<string> { Status = false, Message = $"Email  address  {request.Email} already registered, check and try again later.", StatusCode = StatusEnum.Validation };
        }

        if (_unitOfWork.Users.Any(us => us.phone == request.PhoneNumber))
        {

            return new ApiResponse<string> { Status = false, Message = $"Phone number  {request.PhoneNumber} already registered, check and try again later.", StatusCode = StatusEnum.Validation };
        }

        var createResponse = await _unitOfWork.Users.CreateCustomer(request);
        if (!createResponse.status)
        {

            return new ApiResponse<string> { Status = createResponse.status, Message = createResponse.message, StatusCode = StatusEnum.Validation };
        }

        return new ApiResponse<string> { Status = createResponse.status, Message = createResponse.message, StatusCode = StatusEnum.Success, Data = createResponse.userId ?? "" };
    }

    public async Task<ApiResponse<CustomerLoginResponse>> VerifyUserAccount(string code)
    {
        var loginResponse = await _unitOfWork.Users.VerifyUserAccount(code);
        if (!loginResponse.status)
        {
            return new ApiResponse<CustomerLoginResponse> { Message = loginResponse.message, StatusCode = StatusEnum.Validation, Status = false };
        }
        return new ApiResponse<CustomerLoginResponse> { Message = loginResponse.message, StatusCode = StatusEnum.Success, Status = true, Data = loginResponse.cresponse ?? new CustomerLoginResponse { } };
    }

    public async Task<ApiResponse> ResendEmailVerificationCode(string userId)
    {
        var resendResponse = await _unitOfWork.Users.ResendEmailVerificationCode(userId);

        if (!resendResponse.status)
        {
            return new ApiResponse(resendResponse.message, StatusEnum.Validation, resendResponse.status);
        }
        return new ApiResponse(resendResponse.message, StatusEnum.Success, resendResponse.status);
    }

    public async Task<ApiResponse<CustomerLoginResponse>> LoginUser(LoginRequest request)
    {
        var loginResponse = await _unitOfWork.Users.LoginUser(request);

        if (!loginResponse.status)
        {
            return new ApiResponse<CustomerLoginResponse> { Message = loginResponse.message, StatusCode = StatusEnum.Validation, Status = false };
        }
        return new ApiResponse<CustomerLoginResponse> { Message = loginResponse.message, StatusCode = StatusEnum.Success, Status = true, Data = loginResponse.cresponse ?? new CustomerLoginResponse { } };
    }

    static ApiResponse ValidateCustomerRequest(CreateCustomerRequest request)
    {
        if (string.IsNullOrEmpty(request.FullName))
            return new ApiResponse("First name is required.", StatusEnum.Validation, false);

        else if (string.IsNullOrEmpty(request.Email))
            return new ApiResponse("Email is required.", StatusEnum.Validation, false);

        else if (!request.Email.IsValidEmail())
            return new ApiResponse("Invalid email address.", StatusEnum.Validation, false);

        else if (!request.PhoneNumber.IsValidPhoneNumber())
            return new ApiResponse("Invalid phone number.", StatusEnum.Validation, false);

        else if (!request.Password.Equals(request.ConfirmPassword))
            return new ApiResponse("Passwords do not match.", StatusEnum.Validation, false);
        else
            return new ApiResponse("Validation passed.", StatusEnum.Success, true);
    }
}
