using DaradsHubAPI.Core.IRepository;
using DaradsHubAPI.Core.Model;
using DaradsHubAPI.Core.Model.Request;
using DaradsHubAPI.Core.Model.Response;
using DaradsHubAPI.Core.Services.Interface;
using DaradsHubAPI.Domain.Entities;
using Microsoft.EntityFrameworkCore;
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

    public async Task<ApiResponse<CustomerMetricsResponse>> GetCustomerMetrics()
    {
        var responses = await _unitOfWork.HubUsers.GetCustomerMetrics();
        return new ApiResponse<CustomerMetricsResponse>
        {
            Message = "Successful",
            Status = true,
            Data = responses,
            StatusCode = StatusEnum.Success

        };
    }

    public async Task<ApiResponse<IEnumerable<CustomersListResponse>>> GetCustomers(CustomersListRequest request)
    {
        var query = _unitOfWork.HubUsers.GetCustomers(request);

        var totalProducts = query.Count();
        var paginatedAgents = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize).ToListAsync();

        return new ApiResponse<IEnumerable<CustomersListResponse>> { Message = "Successful", Status = true, Data = paginatedAgents, StatusCode = StatusEnum.Success, TotalRecord = totalProducts, Pages = request.PageSize, CurrentPageCount = request.PageNumber };
    }

    public async Task<ApiResponse> UpdateCustomerStatus(CustomerStatusRequest request)
    {
        var customer = await _unitOfWork.HubUsers.GetSingleWhereAsync(d => d.id == request.CustomerId);

        if (customer is null)
        {
            return new ApiResponse("Customer record not found.", StatusEnum.Validation, false);
        }
        var user = await _unitOfWork.Users.GetAppUser(customer.email);
        if (user is null)
        {
            return new ApiResponse("User record not found.", StatusEnum.Validation, false);
        }

        user.Status = request.EntityStatus;

        customer.status = (int)request.EntityStatus;

        await _unitOfWork.Users.SaveAsync();

        if (request.EntityStatus == EntityStatusEnum.Suspended)
        {
            var suspended = _unitOfWork.Users.SuspendedAgent(new SuspendedAgent
            {
                UserId = request.CustomerId,
                Duration = request.Duration,
                OptionalNote = request.OptionalNote,
                Reason = request.Reason ?? ""
            });
        }
        if (request.EntityStatus == EntityStatusEnum.Blocked)
        {
            var block = _unitOfWork.Users.BlockAgent(new BlockedAgent
            {
                AgentId = request.CustomerId,
                Reason = request.Reason ?? ""
            });
        }
        if (request.EntityStatus == EntityStatusEnum.Active)
        {
            await _unitOfWork.Users.ClearSuspendBlockRecord(request.CustomerId);
        }

        return new ApiResponse("Success", StatusEnum.Success, true);
    }

    public async Task<ApiResponse<ShortCustomerProfileResponse>> GetCustomerProfile(int customerId)
    {
        var profileResponse = await _unitOfWork.HubUsers.GetCustomerProfile(customerId);

        if (!profileResponse.status)
        {
            if (!profileResponse.status)
            {
                return new ApiResponse<ShortCustomerProfileResponse> { Status = profileResponse.status, Message = profileResponse.message, StatusCode = StatusEnum.Validation };
            }
        }
        return new ApiResponse<ShortCustomerProfileResponse> { Status = profileResponse.status, Message = profileResponse.message, StatusCode = StatusEnum.Success, Data = profileResponse.res ?? new ShortCustomerProfileResponse { } };
    }

    public async Task<ApiResponse<IEnumerable<OrderListResponse>>> GetRecentOrders(string email, OrderListRequest request)
    {
        var query = _unitOfWork.Orders.GetOrders(email, request);

        var totalOrders = query.Count();
        var paginatedOrders = await Task.FromResult(query.Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize).ToList());

        return new ApiResponse<IEnumerable<OrderListResponse>> { Message = "Orders fetched successfully.", Status = true, Data = paginatedOrders, StatusCode = StatusEnum.Success, TotalRecord = totalOrders, Pages = request.PageSize, CurrentPageCount = request.PageNumber };
    }

}
