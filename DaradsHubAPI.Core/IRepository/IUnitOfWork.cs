using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaradsHubAPI.Core.IRepository;
public interface IUnitOfWork
{
    IUserRepository Users { get; }
    ICategoryRepository Categories { get; }
    IProductRepository Products { get; }
    IDigitalProductRepository DigitalProducts { get; }
    IWalletTransactionRepository WalletTransactions { get; }
    IOrderRepository Orders { get; }
    IWalletRepository Wallets { get; }
    INotificationRepository Notifications { get; }
}
