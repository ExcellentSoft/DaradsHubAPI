using DaradsHubAPI.Core.Model;
using DaradsHubAPI.Core.Model.Request;

namespace DaradsHubAPI.Core.Services.Interface;

public interface ICategoryService
{
    Task<ApiResponse> CreateCategory(CreateCategoryRequestModel model);
    Task<ApiResponse> CreateUpdateSubCategory(CreateSubCategoryRequestModel model);
    Task<ApiResponse> DeleteCategory(int categoryId);
    Task<ApiResponse> DeleteSubCategory(int subCategoryId);
    Task<ApiResponse<IEnumerable<IdNameRecord>>> GetAgentsLookUp(string? searchText);
    Task<ApiResponse<CategoryResponse>> GetById(int Id);
    Task<ApiResponse<IEnumerable<IdNameRecord>>> GetCatalogues(string? searchText);
    Task<ApiResponse<IEnumerable<CategoryResponse>>> GetCategories(string? searchText);
    Task<ApiResponse<IEnumerable<IdNameRecord>>> GetSubCategories(string? searchText, int categoryId);
    Task<ApiResponse> UpdateCategory(UpdateCategoryRequestModel model);
}