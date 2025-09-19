using DaradsHubAPI.Domain.Entities;

namespace DaradsHubAPI.Core.IRepository;

public interface IWalletTransactionRepository : IGenericRepository<GwalletTran>
{
    IQueryable<AgentWalletTransactionRecord> GetAgentWalletTransactions(string email);
    IQueryable<WalletTransactionRecords> GetWalletTransactions(string email);
}