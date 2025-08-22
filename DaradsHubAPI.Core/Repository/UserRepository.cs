using DaradsHubAPI.Core.IRepository;
using DaradsHubAPI.Domain.Entities;
using DaradsHubAPI.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DaradsHubAPI.Core.Repository;
public class UserRepository(AppDbContext _context, UserManager<User> _userManager, SignInManager<User> _signInManager, IServiceProvider _serviceProvider, IOptionsSnapshot<AppSettings> _optionsSnapshot) : GenericRepository<userstb>(_context), IUserRepository
{

}
