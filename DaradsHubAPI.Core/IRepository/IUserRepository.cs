using DaradsHubAPI.Core.Model.Request;
using DaradsHubAPI.Core.Model.Response;
using DaradsHubAPI.Domain.Entities;

namespace DaradsHubAPI.Core.IRepository;
public interface IUserRepository : IGenericRepository<userstb>
{
    Task<(bool status, string message)> AddAgentByAdmin(AddAgentRequest request, string photoUrl);
    Task AddAgentReview(HubAgentReview model);
    Task BlockAgent(BlockedAgent model);
    Task<(bool status, string message)> ChangePassword(ChangePasswordRequest request, string email);
    Task ClearSuspendBlockRecord(int agentId);
    Task<int> CountCustomers(string audienceType);
    Task<(bool status, string message, string? userId)> CreateAgent(CreateAgentRequest request);
    Task<(bool status, string message, string? userId)> CreateCustomer(CreateCustomerRequest request);
    Task<DashboardMetricsResponse> DashboardMetrics(string email);
    Task<(bool status, string message, string? userId)> ForgetPassword(ForgetPasswordRequest request);
    Task<(bool status, string message, CustomerProfileResponse? res)> GetAdminProfile(string email);
    Task<(bool status, string message, AgentProfileResponse? res)> GetAgentProfile(string email);
    Task<List<MessagesSent>> GetAllSentMessages();
    Task<User?> GetAppUser(string email);
    Task<List<string?>> GetAppUsersEmails(bool isCustomer, bool isAgent);
    Task<MessageAudiences> GetAudiences(string audienceType);
    Task<List<userstb>> GetCustomers(string audienceType, int pageSize, int pageIndex);
    Task<MessageEmailTemplates> GetEmailTemplates(int Id);
    Task<(bool status, string message, CustomerProfileResponse? res)> GetProfile(string email);
    Task<MessagesSent> GetSentMessage(int Id);
    Task<(bool status, string message, CustomerLoginResponse? cresponse)> LoginAdmin(AdminLoginRequest request);
    Task<(bool status, string message, CustomerLoginResponse? cresponse)> LoginUser(LoginRequest request);
    Task<(bool status, string message)> ResendEmailVerificationCode(string userId);
    Task<(bool status, string message)> ResendResetPasswordCode(string userId);
    Task<(bool status, string message)> ResetPassword(ResetPasswordRequest request);
    Task SaveMessageSentLog(MessagesSentLogs entity);
    Task SaveMessagesLogs(List<MessagesSentLogs> entities);
    Task SaveSentMessageDetails(MessagesSent entity);
    Task SuspendedAgent(SuspendedAgent model);
    Task<(bool status, string message)> UpdateAgentProfile(AgentProfileRequest request, string email, string imagePath);
    Task<(bool status, string message)> UpdateProfile(CustomerProfileRequest request, string email, string imagePath);
    Task<(bool status, string message, CustomerLoginResponse? cresponse)> VerifyUserAccount(string code);
}
