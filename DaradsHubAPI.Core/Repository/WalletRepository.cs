using DaradsHubAPI.Core.IRepository;
using DaradsHubAPI.Domain.Entities;
using DaradsHubAPI.Infrastructure;
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
}
