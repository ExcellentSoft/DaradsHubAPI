using DaradsHubAPI.Core.Model;
using DaradsHubAPI.Core.Model.Request;

namespace DaradsHubAPI.Core.Services.Interface;

public interface ICategoryService
{
    Task<ApiResponse> CreateCategory(CreateCategoryRequestModel model);
    Task<ApiResponse> DeleteCategory(int categoryId);
    Task<CategoryResponse> GetById(int Id);
    Task<ApiResponse<IEnumerable<CategoryResponse>>> GetCategories(string? searchText);
    Task<ApiResponse<IEnumerable<IdNameRecord>>> GetSubCategories(string? searchText, int categoryId);
    Task<ApiResponse> UpdateCategory(UpdateCategoryRequestModel model);
}