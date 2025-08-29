using DaradsHubAPI.Core.Model.Response;
using DaradsHubAPI.Domain.Entities;

namespace DaradsHubAPI.Core.IRepository;
public interface IUserRepository : IGenericRepository<userstb>
{
    Task<int> CountCustomers(string audienceType);
    Task<(bool status, string message, string? userId)> CreateCustomer(CreateCustomerRequest request);
    Task<List<MessagesSent>> GetAllSentMessages();
    Task<MessageAudiences> GetAudiences(string audienceType);
    Task<List<userstb>> GetCustomers(string audienceType, int pageSize, int pageIndex);
    Task<MessageEmailTemplates> GetEmailTemplates(int Id);
    Task<MessagesSent> GetSentMessage(int Id);
    Task<(bool status, string message, CustomerLoginResponse? cresponse)> LoginUser(LoginRequest request);
    Task<(bool status, string message)> ResendEmailVerificationCode(string userId);
    Task SaveMessageSentLog(MessagesSentLogs entity);
    Task SaveMessagesLogs(List<MessagesSentLogs> entities);
    Task SaveSentMessageDetails(MessagesSent entity);
    Task<(bool status, string message, CustomerLoginResponse? cresponse)> VerifyUserAccount(string code);
}
