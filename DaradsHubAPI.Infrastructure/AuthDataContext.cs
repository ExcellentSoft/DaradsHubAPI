using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using DaradsHubAPI.Domain.Entities;

namespace DaradsHubAPI.Infrastructure;
public class AuthDataContext(DbContextOptions<AuthDataContext> option) : IdentityDbContext<User>(option)
{ }