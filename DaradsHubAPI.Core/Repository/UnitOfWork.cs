using DaradsHubAPI.Core.IRepository;
using DaradsHubAPI.Domain.Entities;
using DaradsHubAPI.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace DaradsHubAPI.Core.Repository;
public class UnitOfWork(AppDbContext _context, UserManager<User> _userManager, SignInManager<User> _signInManager, IServiceProvider _serviceProvider, IOptionsSnapshot<AppSettings> _optionsSnapshot) : IUnitOfWork, IDisposable
{
    public IUserRepository Users => new UserRepository(_context, _userManager, _signInManager, _serviceProvider, _optionsSnapshot);
    public ICategoryRepository Categories => new CategoryRepository(_context);
    public IProductRepository Products => new ProductRepository(_context);
    public IDigitalProductRepository DigitalProducts => new DigitalProductRepository(_context);
    public IWalletTransactionRepository WalletTransactions => new WalletTransactionRepository(_context);
    public IOrderRepository Orders => new OrderRepository(_context);
    public IWalletRepository Wallets => new WalletRepository(_context);
    public INotificationRepository Notifications => new NotificationRepository(_context);
    public IHubUserRepository HubUsers => new HubUserRepository(_context);

    public void Dispose()
    {
        _context.Dispose();
        GC.SuppressFinalize(this);
    }
}