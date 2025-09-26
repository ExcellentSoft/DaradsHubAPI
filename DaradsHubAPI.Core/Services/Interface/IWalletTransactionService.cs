using DaradsHubAPI.Core.Model;
using DaradsHubAPI.Core.Model.Request;
using DaradsHubAPI.Domain.Entities;

namespace DaradsHubAPI.Core.Services.Interface;
public interface IWalletTransactionService
{
    Task<ApiResponse> CreateWithdrawalRequest(CreateWithdrawalRequest model, string email, int agentId);
    Task<ApiResponse<AgentBalanceResponse>> GetAgentBalance(string email);
    Task<ApiResponse<IEnumerable<AgentWalletTransactionRecord>>> GetAgentWalletTransactions(TransactionListRequest request, string email);
    Task<ApiResponse<IEnumerable<WalletTransactionRecords>>> GetWalletTransactions(TransactionListRequest request, string email);
    Task<ApiResponse<IEnumerable<WithdrawalRequestResponse>>> GetWithdrawalRequests(int agentId);
}
