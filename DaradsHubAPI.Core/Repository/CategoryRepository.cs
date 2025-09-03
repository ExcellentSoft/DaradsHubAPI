using DaradsHubAPI.Core.IRepository;
using DaradsHubAPI.Domain.Entities;
using DaradsHubAPI.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DaradsHubAPI.Core.Repository
{
    public class CategoryRepository(AppDbContext _context) : GenericRepository<category>(_context), ICategoryRepository
    {
        public async Task<category> CreateCategory(category model)
        {
            var newEntity = await _context.categories.AddAsync(model);
            await _context.SaveChangesAsync();
            return newEntity.Entity!;
        }

        public async Task<category?> GetCategoryById(int Id)
        {
            var category = await _context.categories.FirstOrDefaultAsync(s => s.id == Id);
            return category;
        }
        public IQueryable<SubCategory> GetSubCategories(int categoryId)
        {
            var category = _context.SubCategories.Where(s => s.CategoryId == categoryId);
            return category;
        }
    }
}
