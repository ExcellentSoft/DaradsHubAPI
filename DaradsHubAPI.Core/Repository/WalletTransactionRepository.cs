using DaradsHubAPI.Core.IRepository;
using DaradsHubAPI.Domain.Entities;
using DaradsHubAPI.Infrastructure;

namespace DaradsHubAPI.Core.Repository
{
    public class WalletTransactionRepository(AppDbContext _context) : GenericRepository<GwalletTran>(_context), IWalletTransactionRepository
    {
        public IQueryable<WalletTransactionRecords> GetWalletTransactions(string email)
        {

            var transactionRecords = from trans in _context.GwalletTrans.Where(p => p.userName == email)
                                     orderby trans.transdate descending
                                     select new WalletTransactionRecords
                                     {
                                         Id = trans.id,
                                         Amount = trans.amt,
                                         Description = trans.orderItem,
                                         ReferenceNumber = trans.refNo,
                                         Status = "Completed",
                                         TransactionDate = trans.transdate,
                                         Type = trans.DR > 0 ? "Debit" : "Credit"

                                     };
            return transactionRecords;
        }

        public IQueryable<AgentWalletTransactionRecord> GetAgentWalletTransactions(string email)
        {

            var transactionRecords = from trans in _context.GwalletTrans.Where(p => p.userName == email)
                                     orderby trans.transdate descending
                                     select new AgentWalletTransactionRecord
                                     {
                                         Id = trans.id,
                                         Amount = trans.amt,
                                         Balance = trans.walletBal,
                                         ReferenceNumber = trans.refNo,
                                         Status = "Completed",
                                         TransactionDate = trans.transdate,
                                         Type = trans.DR > 0 ? "Debit" : "Credit"

                                     };
            return transactionRecords;
        }
    }
}
