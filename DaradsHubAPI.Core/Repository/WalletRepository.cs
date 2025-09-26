using DaradsHubAPI.Core.IRepository;
using DaradsHubAPI.Core.Model.Request;
using DaradsHubAPI.Domain.Entities;
using DaradsHubAPI.Infrastructure;
using DaradsHubAPI.Shared.Static;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace DaradsHubAPI.Core.Repository;
public class WalletRepository(AppDbContext _context) : GenericRepository<wallettb>(_context), IWalletRepository
{

    public async Task<AgentBalanceResponse> GetAgentWalletBalance(string email)
    {
        var balance = await _context.wallettb.Where(e => e.UserId == email).Select(c => new AgentBalanceResponse
        {
            Balance = c.Balance,
            LastUpdate = c.UpdateDate,

        }).FirstOrDefaultAsync();

        return balance ?? new AgentBalanceResponse { };
    }

    public async Task CreateHubWithdrawalRequest(HubWithdrawalRequest request)
    {
        _context.HubWithdrawalRequests.Add(request);
        await _context.SaveChangesAsync();
    }

    public async Task<List<WithdrawalRequestResponse>> GetWithdrawalRequests(int agentId)
    {
        var requestResponses = await (from req in _context.HubWithdrawalRequests
                                      where req.AgentId == agentId
                                      orderby req.DateCreated descending
                                      select new WithdrawalRequestResponse
                                      {
                                          Id = req.Id,
                                          Amount = req.Amount,
                                          DateCreated = req.DateCreated,
                                          RefNumber = req.ReferenceNumber,
                                          Status = req.Status.GetDescription(),
                                      }).Take(10).ToListAsync();

        return requestResponses;
    }
}
