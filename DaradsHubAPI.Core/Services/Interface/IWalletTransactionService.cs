using DaradsHubAPI.Core.Model;
using DaradsHubAPI.Domain.Entities;

namespace DaradsHubAPI.Core.Services.Interface;
public interface IWalletTransactionService
{
    Task<ApiResponse<IEnumerable<WalletTransactionRecords>>> GetWalletTransactions(TransactionListRequest request, string email);
}
