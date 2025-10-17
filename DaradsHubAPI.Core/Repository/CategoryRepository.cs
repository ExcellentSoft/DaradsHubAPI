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

        public async Task CreateSubCategory(SubCategory model)
        {
            await _context.SubCategories.AddAsync(model);
        }

        public async Task DeleteSubCategories(int categoryId)
        {
            await _context.SubCategories.Where(r => r.CategoryId == categoryId).ExecuteDeleteAsync();
        }
        public async Task DeleteSubCategory(int subCategoryId)
        {
            await _context.SubCategories.Where(r => r.Id == subCategoryId).ExecuteDeleteAsync();
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

        public IQueryable<Catalogue> GetCatalogues()
        {
            var catalogues = _context.Catalogues.AsNoTracking();
            return catalogues;
        }
    }
}
