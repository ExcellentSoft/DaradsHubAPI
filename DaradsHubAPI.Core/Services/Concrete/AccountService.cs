using DaradsHubAPI.Core.IRepository;
using DaradsHubAPI.Core.Model;
using DaradsHubAPI.Core.Model.Response;
using DaradsHubAPI.Core.Services.Interface;
using DaradsHubAPI.Domain.Entities;
using DaradsHubAPI.Shared.Concrete;
using DaradsHubAPI.Shared.Customs;
using DaradsHubAPI.Shared.Extentions;
using DaradsHubAPI.Shared.Interface;
using DaradsHubAPI.Shared.Static;
using static DaradsHubAPI.Domain.Enums.Enum;

namespace DaradsHubAPI.Core.Services.Concrete;
public class AccountService(IUnitOfWork _unitOfWork, IFileService _fileService) : IAccountService
{
    public async Task<ApiResponse<DashboardMetricsResponse>> DashboardMetrics(string email)
    {
        var response = await _unitOfWork.Users.DashboardMetrics(email);

        return new ApiResponse<DashboardMetricsResponse> { Status = true, Message = "Successful.", StatusCode = StatusEnum.Success, Data = response };
    }

    public async Task<ApiResponse<CustomerProfileResponse>> GetCustomerProfile(string email)
    {
        var profileResponse = await _unitOfWork.Users.GetProfile(email);

        if (!profileResponse.status)
        {
            if (!profileResponse.status)
            {

                return new ApiResponse<CustomerProfileResponse> { Status = profileResponse.status, Message = profileResponse.message, StatusCode = StatusEnum.Validation };
            }
        }

        return new ApiResponse<CustomerProfileResponse> { Status = profileResponse.status, Message = profileResponse.message, StatusCode = StatusEnum.Success, Data = profileResponse.res ?? new CustomerProfileResponse { } };
    }

    public async Task<ApiResponse<CustomerProfileResponse>> GetAdminProfile(string email)
    {
        var profileResponse = await _unitOfWork.Users.GetProfile(email);

        if (!profileResponse.status)
        {
            if (!profileResponse.status)
            {

                return new ApiResponse<CustomerProfileResponse> { Status = profileResponse.status, Message = profileResponse.message, StatusCode = StatusEnum.Validation };
            }
        }

        return new ApiResponse<CustomerProfileResponse> { Status = profileResponse.status, Message = profileResponse.message, StatusCode = StatusEnum.Success, Data = profileResponse.res ?? new CustomerProfileResponse { } };
    }

    public async Task<ApiResponse<AgentProfileResponse>> GetAgentProfile(string email)
    {
        var profileResponse = await _unitOfWork.Users.GetAgentProfile(email);

        if (!profileResponse.status)
        {
            if (!profileResponse.status)
            {

                return new ApiResponse<AgentProfileResponse> { Status = profileResponse.status, Message = profileResponse.message, StatusCode = StatusEnum.Validation };
            }
        }

        return new ApiResponse<AgentProfileResponse> { Status = profileResponse.status, Message = profileResponse.message, StatusCode = StatusEnum.Success, Data = profileResponse.res ?? new AgentProfileResponse { } };
    }

    public async Task<ApiResponse> UpdateProfile(CustomerProfileRequest request, string email)
    {
        if (string.IsNullOrEmpty(request.PhoneNumber))
            return new ApiResponse("Phone number is required.", StatusEnum.Validation, false);
        if (string.IsNullOrEmpty(request.FullName))
            return new ApiResponse("User name is required.", StatusEnum.Validation, false);

        if (!StringExtensions.IsValidPhoneNumber(request.PhoneNumber.Trim()))
            return new ApiResponse("Phone number is not valid, check and try again.", StatusEnum.Validation, false);

        var photoPath = "";
        if (request.Photo is not null)
        {
            var maxUploadSize = 5;
            var allowedExtensions = new[] { ".png", ".jpg", ".jpeg", ".jpe", ".gif" };
            if (request.Photo.Length > (maxUploadSize * 1024 * 1024))
                return new ApiResponse($"Max upload size exceeded. Max size is {maxUploadSize}MB", StatusEnum.Validation, false);

            var ext = Path.GetExtension(request.Photo.FileName);
            if (!allowedExtensions.Contains(ext))
                return new ApiResponse($"Invalid file format. Supported file formats include {string.Join(", ", allowedExtensions)}", StatusEnum.Validation, false);
            var fileResponse = await _fileService.AddPhoto(request.Photo, GenericStrings.PROFILE_IMAGES_FOLDER_NAME);
            Uri url = fileResponse.SecureUrl;
            if (!string.IsNullOrEmpty(url.AbsoluteUri))
            {
                photoPath = url.AbsoluteUri;
            }
        }

        var updateProfileResponse = await _unitOfWork.Users.UpdateProfile(request, email, photoPath);

        if (!updateProfileResponse.status)
        {
            return new ApiResponse(updateProfileResponse.message, StatusEnum.Validation, updateProfileResponse.status);
        }
        return new ApiResponse(updateProfileResponse.message, StatusEnum.Success, updateProfileResponse.status);
    }
    public async Task<ApiResponse> UpdateAgentProfile(AgentProfileRequest request, string email)
    {
        if (string.IsNullOrEmpty(request.PhoneNumber))
            return new ApiResponse("Phone number is required.", StatusEnum.Validation, false);
        if (string.IsNullOrEmpty(request.FullName))
            return new ApiResponse("User name is required.", StatusEnum.Validation, false);

        if (!StringExtensions.IsValidPhoneNumber(request.PhoneNumber.Trim()))
            return new ApiResponse("Phone number is not valid, check and try again.", StatusEnum.Validation, false);

        var photoPath = "";
        if (request.Photo is not null)
        {
            var maxUploadSize = 5;
            var allowedExtensions = new[] { ".png", ".jpg", ".jpeg", ".jpe", ".gif" };
            if (request.Photo.Length > (maxUploadSize * 1024 * 1024))
                return new ApiResponse($"Max upload size exceeded. Max size is {maxUploadSize}MB", StatusEnum.Validation, false);

            var ext = Path.GetExtension(request.Photo.FileName);
            if (!allowedExtensions.Contains(ext))
                return new ApiResponse($"Invalid file format. Supported file formats include {string.Join(", ", allowedExtensions)}", StatusEnum.Validation, false);
            var fileResponse = await _fileService.AddPhoto(request.Photo, GenericStrings.PROFILE_IMAGES_FOLDER_NAME);
            Uri url = fileResponse.SecureUrl;
            if (!string.IsNullOrEmpty(url.AbsoluteUri))
            {
                photoPath = url.AbsoluteUri;
            }
        }

        var updateProfileResponse = await _unitOfWork.Users.UpdateAgentProfile(request, email, photoPath);

        if (!updateProfileResponse.status)
        {
            return new ApiResponse(updateProfileResponse.message, StatusEnum.Validation, updateProfileResponse.status);
        }
        return new ApiResponse(updateProfileResponse.message, StatusEnum.Success, updateProfileResponse.status);
    }


    public async Task<ApiResponse> ChangePassword(ChangePasswordRequest request, string email)
    {
        var changePasswordResponse = await _unitOfWork.Users.ChangePassword(request, email);

        if (!changePasswordResponse.status)
        {
            return new ApiResponse(changePasswordResponse.message, StatusEnum.Validation, changePasswordResponse.status);
        }
        return new ApiResponse(changePasswordResponse.message, StatusEnum.Success, changePasswordResponse.status);
    }

    public async Task<ApiResponse> SubmitCashPay(SubmitCashPayRequest r)
    {
        
        var entity = new CashPayment
        {
            DepositorName=r.DepositorName, Amount =r.Amount,PayDate=GetLocalDateTime.CurrentDateTime(),
            BankName = r.BankName,
            PaidFromAccountName = r.PaidFromAccountName,
            PhoneNumber = r.PhoneNumber,
            Status = "Submitted",
            WalletUserId = r.UserId,
          //  UpdateDate = GetLocalDateTime.CurrentDateTime()
        };
        await _unitOfWork.Users.SubmitCashPayment(entity);
        return new ApiResponse("Your payment details submitted",StatusEnum.Success,true);
    }
}
