using DaradsHubAPI.Core.IRepository;
using DaradsHubAPI.Core.Model;
using DaradsHubAPI.Core.Model.Request;
using DaradsHubAPI.Core.Services.Interface;
using DaradsHubAPI.Domain.Entities;
using DaradsHubAPI.Shared.Interface;
using DaradsHubAPI.Shared.Static;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using static DaradsHubAPI.Domain.Enums.Enum;

namespace DaradsHubAPI.Core.Services.Concrete;

public class CategoryService(IUnitOfWork _unitOfWork, IFileService _fileService) : ICategoryService
{
    public async Task<ApiResponse> CreateCategory(CreateCategoryRequestModel model)
    {
        string icon = "";
        if (string.IsNullOrEmpty(model.Name))
            return new ApiResponse("Category name is required.", StatusEnum.Validation, false);

        if (_unitOfWork.Categories.Any(r => r.name == model.Name))
        {
            return new ApiResponse("Category name already  exist.", StatusEnum.Validation, false);
        }

        if (model.Icon is not null)
        {
            var (status, msg, path) = await SaveCategoryImage(model.Icon);
            if (!status)
                return new ApiResponse(msg, StatusEnum.Validation, false);
            icon = path ?? "";
        }
        var newCategory = new category
        {
            name = model.Name,
            Description = model.Description,
            icon = icon
        };
        await _unitOfWork.Categories.CreateCategory(newCategory);

        return new ApiResponse("Category created successfully.", StatusEnum.Success, true);
    }

    public async Task<ApiResponse> UpdateCategory(UpdateCategoryRequestModel model)
    {
        string icon = "";
        var category = await _unitOfWork.Categories.GetSingleWhereAsync(cat => cat.id == model.Id);
        if (category is null)
        {
            return new ApiResponse("Category record not found.", StatusEnum.Validation, false);
        }

        if (string.IsNullOrEmpty(model.Name))
            return new ApiResponse("Category name is required.", StatusEnum.Validation, false);

        if (_unitOfWork.Categories.Any(r => r.name == model.Name && r.id != model.Id))
        {
            return new ApiResponse("Category name already  exist.", StatusEnum.Validation, false);
        }

        if (model.Icon is not null)
        {
            var (status, msg, path) = await SaveCategoryImage(model.Icon);
            if (!status)
                return new ApiResponse(msg, StatusEnum.Validation, false);
            icon = path ?? "";
        }

        category.name = model.Name;
        category.Description = model.Description;
        category.icon = string.IsNullOrEmpty(icon) ? category.icon : icon;

        await _unitOfWork.Categories.Update(category);

        return new ApiResponse("Category updated successfully.", StatusEnum.Success, true);
    }

    public async Task<ApiResponse> CreateUpdateSubCategory(CreateSubCategoryRequestModel model)
    {
        var category = await _unitOfWork.Categories.GetSingleWhereAsync(cat => cat.id == model.CategoryId);
        if (category is null)
        {
            return new ApiResponse("Category record not found.", StatusEnum.Validation, false);
        }
        if (model.SubCategoryNames.Any())
        {
            await _unitOfWork.Categories.DeleteSubCategories(category.id);
            foreach (var item in model.SubCategoryNames)
            {
                var sub = new SubCategory
                {
                    CategoryId = category.id,
                    Name = item,
                };
                await _unitOfWork.Categories.CreateSubCategory(sub);
            }
        }

        await _unitOfWork.Categories.SaveAsync();
        return new ApiResponse("Successful.", StatusEnum.Success, true);
    }

    public async Task<ApiResponse> DeleteCategory(int categoryId)
    {
        await _unitOfWork.Categories.DeleteSubCategories(categoryId);
        await _unitOfWork.Categories.DeleteWhere(cat => cat.id == categoryId);
        return new ApiResponse("Category deleted successfully.", StatusEnum.Success, true);
    }

    public async Task<ApiResponse> DeleteSubCategory(int subCategoryId)
    {
        await _unitOfWork.Categories.DeleteSubCategory(subCategoryId);

        return new ApiResponse("Sub category deleted successfully.", StatusEnum.Success, true);
    }

    public async Task<ApiResponse<CategoryResponse>> GetById(int Id)
    {
        var category = await _unitOfWork.Categories.GetSingleWhereAsync(s => s.id == Id);
        if (category is not null)
        {
            var newCategory = new CategoryResponse
            {
                Description = category.Description,
                Name = category.name,
                Icon = category.icon,
                Id = category.id,
                SubCategoryData = _unitOfWork.Categories.GetSubCategories(Id).Select(e => new SubCategoryDatum
                {
                    Id = e.Id,
                    Name = e.Name
                }).ToList()
            };
            return new ApiResponse<CategoryResponse> { Data = newCategory, Message = "Successful", StatusCode = StatusEnum.Success, Status = true };

        }
        return new ApiResponse<CategoryResponse> { Message = "record not found", StatusCode = StatusEnum.NoRecordFound, Status = false };
    }

    public async Task<ApiResponse<IEnumerable<CategoryResponse>>> GetCategories(string? searchText)
    {
        searchText = searchText?.Trim().ToLower();
        var categories = _unitOfWork.Categories.GetWhere(cat => searchText == null || cat.name.ToLower().Contains(searchText)).Select(c => new CategoryResponse
        {
            Description = c.Description,
            Icon = c.icon,
            Id = c.id,
            Name = c.name
        }).ToList();

        return await Task.FromResult(new ApiResponse<IEnumerable<CategoryResponse>> { Data = categories, Message = "Successful", Status = true, StatusCode = StatusEnum.Success });
    }

    public async Task<ApiResponse<IEnumerable<IdNameRecord>>> GetCatalogues(string? searchText)
    {
        searchText = searchText?.Trim().ToLower();
        var catalogues = _unitOfWork.Categories.GetCatalogues();

        if (!string.IsNullOrWhiteSpace(searchText))
        {
            catalogues = catalogues.Where(s => s.Name.ToLower().Contains(searchText));
        }

        var iCatalogues = await catalogues.Select(c => new IdNameRecord
        {
            Id = c.Id,
            Name = c.Name
        }).ToListAsync();

        return await Task.FromResult(new ApiResponse<IEnumerable<IdNameRecord>> { Data = iCatalogues, Message = "Successful", Status = true, StatusCode = StatusEnum.Success });
    }

    public async Task<ApiResponse<IEnumerable<IdNameRecord>>> GetSubCategories(string? searchText, int categoryId)
    {
        searchText = searchText?.Trim().ToLower();

        var categories = _unitOfWork.Categories.GetSubCategories(categoryId);

        if (!string.IsNullOrWhiteSpace(searchText))
        {
            categories = categories.Where(s => s.Name.ToLower().Contains(searchText));
        }

        var iCategories = await categories.Select(c => new IdNameRecord
        {
            Id = c.Id,
            Name = c.Name
        }).ToListAsync();

        return new ApiResponse<IEnumerable<IdNameRecord>> { Data = iCategories, Message = "Successful", Status = true, StatusCode = StatusEnum.Success };
    }

    public async Task<ApiResponse<IEnumerable<IdNameRecord>>> GetAgentsLookUp(string? searchText)
    {
        searchText = searchText?.Trim().ToLower();

        var agent = _unitOfWork.Users.GetWhere(f => f.IsAgent == true);

        if (!string.IsNullOrWhiteSpace(searchText))
        {
            agent = agent.Where(s => s.fullname.ToLower().Contains(searchText));
        }

        var iAgent = agent.Select(c => new IdNameRecord
        {
            Id = c.id,
            Name = c.fullname

        }).ToList();

        return await Task.FromResult(new ApiResponse<IEnumerable<IdNameRecord>> { Data = iAgent, Message = "Successful", Status = true, StatusCode = StatusEnum.Success });
    }



    private async Task<(bool status, string msg, string? path)> SaveCategoryImage(IFormFile file)
    {
        var photoPath = "";

        var maxUploadSize = 5;
        var allowedExtensions = new[] { ".png", ".jpg", ".jpeg", ".jpe", ".gif" };
        if (file.Length > maxUploadSize * 1024 * 1024)
            return (false, $"Max upload size exceeded. Max size is {maxUploadSize}MB", null);

        var ext = Path.GetExtension(file.FileName);
        if (!allowedExtensions.Contains(ext))
            return new(false, $"Invalid file format. Supported file formats include {string.Join(", ", allowedExtensions)}", null);
        var fileResponse = await _fileService.AddPhoto(file, GenericStrings.CATEGORY_IMAGES_FOLDER_NAME);
        Uri url = fileResponse.SecureUrl;
        if (!string.IsNullOrEmpty(url.AbsoluteUri))
        {
            photoPath = url.AbsoluteUri;

        }
        return new(true, "Category image saved successfully.", photoPath);
    }
}