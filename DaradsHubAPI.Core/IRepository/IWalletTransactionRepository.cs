using DaradsHubAPI.Domain.Entities;

namespace DaradsHubAPI.Core.IRepository;

public interface IWalletTransactionRepository : IGenericRepository<GwalletTran>
{
    IQueryable<WalletTransactionRecords> GetWalletTransactions(string email);
}