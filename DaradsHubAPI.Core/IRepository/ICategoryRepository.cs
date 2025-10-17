using DaradsHubAPI.Domain.Entities;

namespace DaradsHubAPI.Core.IRepository;

public interface ICategoryRepository : IGenericRepository<category>
{
    Task<category?> GetCategoryById(int Id);
    Task<category> CreateCategory(category model);
    IQueryable<SubCategory> GetSubCategories(int categoryId);
    IQueryable<Catalogue> GetCatalogues();
    Task CreateSubCategory(SubCategory model);
    Task DeleteSubCategories(int categoryId);
    Task DeleteSubCategory(int subCategoryId);
}