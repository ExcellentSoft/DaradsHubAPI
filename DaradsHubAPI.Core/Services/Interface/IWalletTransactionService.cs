using DaradsHubAPI.Core.Model;
using DaradsHubAPI.Domain.Entities;

namespace DaradsHubAPI.Core.Services.Interface;
public interface IWalletTransactionService
{
    Task<ApiResponse<AgentBalanceResponse>> GetAgentBalance(string email);
    Task<ApiResponse<IEnumerable<AgentWalletTransactionRecord>>> GetAgentWalletTransactions(TransactionListRequest request, string email);
    Task<ApiResponse<IEnumerable<WalletTransactionRecords>>> GetWalletTransactions(TransactionListRequest request, string email);
}
