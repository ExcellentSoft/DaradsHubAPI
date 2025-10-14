using DaradsHubAPI.Core.IRepository;
using DaradsHubAPI.Core.Model;
using DaradsHubAPI.Core.Model.Request;
using DaradsHubAPI.Core.Model.Response;
using DaradsHubAPI.Core.Services.Interface;
using DaradsHubAPI.Domain.Entities;
using DaradsHubAPI.Shared.Customs;
using DaradsHubAPI.Shared.Interface;
using DaradsHubAPI.Shared.Static;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using static DaradsHubAPI.Domain.Enums.Enum;

namespace DaradsHubAPI.Core.Services.Concrete;
public class DigitalProductService(IUnitOfWork _unitOfWork, IFileService _fileService) : IDigitalProductService
{
    public async Task<ApiResponse<IEnumerable<IdNameRecord>>> GetDigitalProducts(string? searchText, int agentId)
    {
        var products = _unitOfWork.DigitalProducts.GetDigitalProducts(searchText, agentId);
        var iProducts = await products.Select(c => new IdNameRecord
        {
            Id = c.Id,
            Name = c.Name
        }).ToListAsync();
        return new ApiResponse<IEnumerable<IdNameRecord>> { Data = iProducts, Message = "Successful", Status = true, StatusCode = StatusEnum.Success };
    }

    public async Task<ApiResponse<DigitalHubProductResponse>> GetDigitalProduct(long productId)
    {
        var product = await _unitOfWork.DigitalProducts.GetSingleWhereAsync(e => e.Id == productId);
        if (product is null)
        {
            return new ApiResponse<DigitalHubProductResponse> { Message = "Product record not found", Status = false, StatusCode = StatusEnum.NoRecordFound };
        }
        var response = new DigitalHubProductResponse
        {
            Title = product.Title,
            CatalogueId = product.CatalogueId,
            Value = product.Value,
            Description = product.Description,
            DiscountPrice = product.DiscountPrice,
            Price = product.Price
        };

        response.Images = await _unitOfWork.DigitalProducts.GetDigitalProductImages(productId);

        return new ApiResponse<DigitalHubProductResponse> { Data = response, Message = "Successful", Status = true, StatusCode = StatusEnum.Success };
    }

    public async Task<ApiResponse> AddDigitalProduct(AddDigitalHubProductRequest model, string email)
    {
        var user = await _unitOfWork.Users.GetSingleWhereAsync(d => d.email == email && d.IsAgent == true);
        var prod = new HubDigitalProduct
        {
            CatalogueId = model.CatalogueId,
            Title = model.Title,
            DateCreated = DateTime.Now,
            Description = model.Description,
            Price = model.Price,
            DiscountPrice = model.DiscountPrice,
            DateUpdated = DateTime.Now,
            AgentId = user!.id
        };
        await _unitOfWork.DigitalProducts.Insert(prod);

        if (model.Images is not null)
        {
            foreach (var image in model.Images)
            {
                var (status, msg, path) = await SaveProductImage(image);
                if (status)
                {
                    var productImage = new DigitalProductImages
                    {
                        ImageUrl = path ?? "",
                        ProductId = prod.Id,
                        Status = true
                    };
                    await _unitOfWork.DigitalProducts.AddHubDigitalProductImages(productImage);
                }
            }

        }
        if (model.Values.Any())
        {
            foreach (var item in model.Values)
            {
                var value = new HubDigitalProductValueLog
                {
                    ProductValue = item,
                    AgentId = user!.id,
                    CatalogueId = model.CatalogueId,
                    IsAvailable = true,
                    DateCreated = GetLocalDateTime.CurrentDateTime(),
                    DateUpdated = GetLocalDateTime.CurrentDateTime(),
                };
                _unitOfWork.DigitalProducts.AddHubDigitalProductValue(value);
            }
            await _unitOfWork.DigitalProducts.SaveAsync();
        }

        return new ApiResponse("Digital product created successfully.", StatusEnum.Success, true);
    }
    public async Task<ApiResponse> UpdateDigitalProduct(UpdateDigitalHubProductRequest model, string email)
    {
        var user = await _unitOfWork.Users.GetSingleWhereAsync(d => d.email == email);
        var product = await _unitOfWork.DigitalProducts.GetSingleWhereAsync(p => p.Id == model.Id && p.AgentId == user!.id);

        if (product is null)
            return new ApiResponse("Product record not found.", StatusEnum.Validation, false);

        product.CatalogueId = model.CatalogueId;
        product.Title = model.Title;
        product.Description = model.Description;
        product.Price = model.Price;
        product.DiscountPrice = model.DiscountPrice;
        product.DateUpdated = DateTime.Now;

        if (model.Images is not null)
        {
            foreach (var image in model.Images)
            {
                var (status, msg, path) = await SaveProductImage(image);
                if (status)
                {
                    var productImage = new DigitalProductImages
                    {
                        ImageUrl = path ?? "",
                        ProductId = product.Id,
                        Status = true
                    };
                    await _unitOfWork.DigitalProducts.AddHubDigitalProductImages(productImage);
                }
            }
        }
        if (model.Values.Any())
        {
            foreach (var item in model.Values)
            {
                var value = new HubDigitalProductValueLog
                {
                    ProductValue = item,
                    AgentId = user!.id,
                    CatalogueId = model.CatalogueId,
                    IsAvailable = true,
                    DateCreated = GetLocalDateTime.CurrentDateTime(),
                    DateUpdated = GetLocalDateTime.CurrentDateTime(),
                };
                _unitOfWork.DigitalProducts.AddHubDigitalProductValue(value);
            }
            await _unitOfWork.DigitalProducts.SaveAsync();
        }

        return new ApiResponse("Digital product updated successfully.", StatusEnum.Success, true);
    }

    public async Task<ApiResponse<IEnumerable<LandingPageDigitalProductResponse>>> GetLandPageProducts()
    {
        var responses = await _unitOfWork.DigitalProducts.GetLandPageProducts().Take(30).ToListAsync();

        return new ApiResponse<IEnumerable<LandingPageDigitalProductResponse>> { Message = "Successful", Status = true, Data = responses, StatusCode = StatusEnum.Success };
    }

    public async Task<ApiResponse<IEnumerable<LandingPageDigitalProductResponse>>> GetPublicLandPageProducts()
    {
        var responses = await _unitOfWork.DigitalProducts.GetPublicLandPageProducts().Take(30).ToListAsync();

        return new ApiResponse<IEnumerable<LandingPageDigitalProductResponse>> { Message = "Successful", Status = true, Data = responses, StatusCode = StatusEnum.Success };
    }

    public async Task<ApiResponse<AgentProductProfileResponse>> GetAgentDigitalProductProfile(int agentId)
    {
        var responses = await _unitOfWork.DigitalProducts.GetAgentDigitalProductProfile(agentId);

        return new ApiResponse<AgentProductProfileResponse> { Message = "Successful", Status = true, Data = responses, StatusCode = StatusEnum.Success };
    }

    public async Task<ApiResponse<AgentProductProfileResponse>> GetPublicAgentDigitalProductProfile(int agentId)
    {
        var responses = await _unitOfWork.DigitalProducts.GetPublicAgentDigitalProductProfile(agentId);

        return new ApiResponse<AgentProductProfileResponse> { Message = "Successful", Status = true, Data = responses, StatusCode = StatusEnum.Success };
    }

    public async Task<ApiResponse<IEnumerable<AgentsProfileResponse>>> GetDigitalAgents(AgentsProfileListRequest request)
    {
        var query = _unitOfWork.DigitalProducts.GetDigitalAgents(request);

        var totalProducts = query.Count();
        var paginatedAgents = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize).ToListAsync();

        return new ApiResponse<IEnumerable<AgentsProfileResponse>> { Message = "Successful", Status = true, Data = paginatedAgents, StatusCode = StatusEnum.Success, TotalRecord = totalProducts, Pages = request.PageSize, CurrentPageCount = request.PageNumber };
    }

    public async Task<ApiResponse<IEnumerable<AgentsProfileResponse>>> GetDigitalPublicAgents(AgentsProfileListRequest request)
    {
        var query = _unitOfWork.DigitalProducts.GetDigitalPublicAgents(request);

        var totalProducts = query.Count();
        var paginatedAgents = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize).ToListAsync();

        return new ApiResponse<IEnumerable<AgentsProfileResponse>> { Message = "Successful", Status = true, Data = paginatedAgents, StatusCode = StatusEnum.Success, TotalRecord = totalProducts, Pages = request.PageSize, CurrentPageCount = request.PageNumber };
    }

    public async Task<ApiResponse<IEnumerable<DigitalProductDetailsResponse>>> GetAgentProducts(AgentDigitalProductListRequest request)
    {
        var query = _unitOfWork.DigitalProducts.GetAgentDigitalProducts(request.CatalogueId, request.AgentId);

        if (!string.IsNullOrWhiteSpace(request.SearchText))
        {
            query = query
                .Where(s => s.Title != null && s.Title.ToLower().Contains(request.SearchText.ToLower()) || s.Name != null && s.Name.ToLower().Contains(request.SearchText.ToLower()));
        }
        var totalProducts = query.Count();
        var paginatedProducts = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize).ToListAsync();


        return new ApiResponse<IEnumerable<DigitalProductDetailsResponse>> { Message = "Successful", Status = true, Data = paginatedProducts, StatusCode = StatusEnum.Success, TotalRecord = totalProducts, Pages = request.PageSize, CurrentPageCount = request.PageNumber };
    }

    public async Task<ApiResponse<IEnumerable<DigitalProductDetailsResponse>>> GetPublicAgentProducts(AgentDigitalProductListRequest request)
    {
        var query = _unitOfWork.DigitalProducts.GetPublicAgentDigitalProducts(request.CatalogueId, request.AgentId);

        if (!string.IsNullOrWhiteSpace(request.SearchText))
        {
            query = query
                .Where(s => s.Title != null && s.Title.ToLower().Contains(request.SearchText.ToLower()) || s.Name != null && s.Name.ToLower().Contains(request.SearchText.ToLower()));
        }
        var totalProducts = query.Count();
        var paginatedProducts = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize).ToListAsync();


        return new ApiResponse<IEnumerable<DigitalProductDetailsResponse>> { Message = "Successful", Status = true, Data = paginatedProducts, StatusCode = StatusEnum.Success, TotalRecord = totalProducts, Pages = request.PageSize, CurrentPageCount = request.PageNumber };
    }
    public async Task<ApiResponse<DigitalProductDetailResponse>> GetAgentProduct(int productId)
    {
        var responses = await _unitOfWork.DigitalProducts.GetAgentDigitalProduct(productId);

        return new ApiResponse<DigitalProductDetailResponse> { Message = "Successful", Status = true, Data = responses, StatusCode = StatusEnum.Success };
    }

    private async Task<(bool status, string msg, string? path)> SaveProductImage(IFormFile file)
    {
        var photoPath = "";

        var maxUploadSize = 5;
        var allowedExtensions = new[] { ".png", ".jpg", ".jpeg", ".jpe", ".gif" };
        if (file.Length > (maxUploadSize * 1024 * 1024))
            return new(false, $"Max upload size exceeded. Max size is {maxUploadSize}MB", null);

        var ext = Path.GetExtension(file.FileName);
        if (!allowedExtensions.Contains(ext))
            return new(false, $"Invalid file format. Supported file formats include {string.Join(", ", allowedExtensions)}", null);
        var fileResponse = await _fileService.AddPhoto(file, GenericStrings.PRODUCT_IMAGES_FOLDER_NAME);
        Uri url = fileResponse.SecureUrl;
        if (!string.IsNullOrEmpty(url.AbsoluteUri))
        {
            photoPath = url.AbsoluteUri;

        }
        return new(true, "Product image saved successfully.", photoPath);
    }
}
