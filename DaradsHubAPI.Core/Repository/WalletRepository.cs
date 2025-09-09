using DaradsHubAPI.Core.IRepository;
using DaradsHubAPI.Domain.Entities;
using DaradsHubAPI.Infrastructure;

namespace DaradsHubAPI.Core.Repository;
public class WalletRepository(AppDbContext _context) : GenericRepository<wallettb>(_context), IWalletRepository
{
}
