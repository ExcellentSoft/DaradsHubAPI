using Microsoft.EntityFrameworkCore;

namespace DaradsHubAPI.Infrastructure;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{

}
