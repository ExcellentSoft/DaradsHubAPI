using DaradsHubAPI.Core.IRepository;
using DaradsHubAPI.Core.Model;
using DaradsHubAPI.Core.Model.Request;
using DaradsHubAPI.Core.Model.Response;
using DaradsHubAPI.Core.Services.Interface;
using DaradsHubAPI.Domain.Entities;
using DaradsHubAPI.Shared.Interface;
using DaradsHubAPI.Shared.Static;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using System;
using static DaradsHubAPI.Domain.Enums.Enum;

namespace DaradsHubAPI.Core.Services.Concrete;
public class ProductService(IUnitOfWork _unitOfWork, IFileService _fileService) : IProductService
{

    public async Task<ApiResponse<IEnumerable<IdNameRecord>>> GetHubProducts(string? searchText)
    {
        var products = _unitOfWork.Products.GetHubProducts(searchText);
        var iCategories = await products.Select(c => new IdNameRecord
        {
            Id = c.Id,
            Name = c.Name
        }).ToListAsync();
        return new ApiResponse<IEnumerable<IdNameRecord>> { Data = iCategories, Message = "Successful", Status = true, StatusCode = StatusEnum.Success };
    }

    public async Task<ApiResponse<IEnumerable<IdNameRecord>>> GetAgent(string? searchText)
    {
        var products = _unitOfWork.Products.GetHubProducts(searchText);
        var iCategories = await products.Select(c => new IdNameRecord
        {
            Id = c.Id,
            Name = c.Name
        }).ToListAsync();
        return new ApiResponse<IEnumerable<IdNameRecord>> { Data = iCategories, Message = "Successful", Status = true, StatusCode = StatusEnum.Success };
    }

    public async Task<ApiResponse> CreateProductRequest(CreateHubProductRequest model, string email)
    {
        var user = await _unitOfWork.Users.GetSingleWhereAsync(d => d.email == email);
        var request = new HubProductRequest
        {
            AgentId = model.AgentId,
            Budget = model.Budget,
            DateCreated = DateTime.Now,
            Description = model.Description,
            CustomerNeed = model.CustomerNeed,
            Location = model.Location,
            IsUrgent = model.IsUrgent,
            ProductRequestTypeEnum = model.ProductRequestTypeEnum,
            PreferredDate = model.PreferredDate,
            Quantity = model.Quantity,
            CustomerId = user!.id
        };
        await _unitOfWork.Products.CreateHubProductRequest(request);

        if (model.ReferenceFiles is not null)
        {
            foreach (var image in model.ReferenceFiles)
            {
                var (status, msg, path) = await SaveProductImage(image);
                if (status)
                {
                    var productImage = new ProductRequestImages
                    {
                        ImageUrl = path ?? "",
                        RequestId = request.Id
                    };
                    await _unitOfWork.Products.AddHubProductRequestImages(productImage);
                }
            }
            await _unitOfWork.Products.SaveAsync();
        }


        return new ApiResponse("Product request created successfully.", StatusEnum.Success, true);
    }

    public async Task<ApiResponse> AddProduct(AddAgentHubProductRequest model, string email)
    {
        var user = await _unitOfWork.Users.GetSingleWhereAsync(d => d.email == email);
        var prod = new HubAgentProduct
        {
            CategoryId = model.CategoryId,
            SubCategoryId = model.SubCategoryId,
            DateCreated = DateTime.Now,
            Description = model.Description,
            ProductId = model.ProductId,
            Price = model.Price,
            DeliveryPrice = model.DeliveryPrice,
            DiscountPrice = model.DiscountPrice,
            SKU = model.SKU,
            IsFreeShipping = model.IsFreeShipping,
            DateUpdated = DateTime.Now,
            AgentId = user!.id,
            Caption = model.Caption,
            EstimateDeliveryTime = model.EstimateDeliveryTime
        };
        await _unitOfWork.Products.Insert(prod);

        if (model.Images.Any())
        {
            foreach (var image in model.Images)
            {
                var (status, msg, path) = await SaveProductImage(image);
                if (status)
                {
                    var productImage = new ProductImages
                    {
                        ImageUrl = path ?? "",
                        ProductId = prod.Id
                    };
                    await _unitOfWork.Products.AddHubProductImages(productImage);
                }
            }
            await _unitOfWork.Products.SaveAsync();
        }

        return new ApiResponse("Product created successfully.", StatusEnum.Success, true);
    }

    public async Task<ApiResponse> UpdateProduct(UpdateAgentHubProductRequest model, string email)
    {
        var user = await _unitOfWork.Users.GetSingleWhereAsync(d => d.email == email);
        var product = await _unitOfWork.Products.GetSingleWhereAsync(p => p.Id == model.Id && p.AgentId == user!.id);

        if (product is null)
            return new ApiResponse("Product record not found.", StatusEnum.Validation, false);

        product.CategoryId = model.CategoryId;
        product.SubCategoryId = model.SubCategoryId;
        product.DateCreated = DateTime.Now;
        product.Description = model.Description;
        product.ProductId = model.ProductId;
        product.Price = model.Price;
        product.DeliveryPrice = model.DeliveryPrice;
        product.DiscountPrice = model.DiscountPrice;
        product.SKU = model.SKU;
        product.IsFreeShipping = model.IsFreeShipping;
        product.DateUpdated = DateTime.Now;
        product.Caption = model.Caption;
        product.EstimateDeliveryTime = model.EstimateDeliveryTime;

        if (model.Images.Any())
        {
            foreach (var image in model.Images)
            {
                var (status, msg, path) = await SaveProductImage(image);
                if (status)
                {
                    var productImage = new ProductImages
                    {
                        ImageUrl = path ?? "",
                        ProductId = product.Id
                    };
                    await _unitOfWork.Products.AddHubProductImages(productImage);
                }
            }
            await _unitOfWork.Products.SaveAsync();
        }

        return new ApiResponse("Product updated successfully.", StatusEnum.Success, true);
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

    public async Task<ApiResponse> AddReview(AddReviewRequestModel model, int userId, bool isDigital)
    {
        var product = await _unitOfWork.Products.GetSingleWhereAsync(x => x.Id == model.productId);
        if (product == null)
            return new ApiResponse($"Product record not found", StatusEnum.NoRecordFound, false);

        var review = new HubReview
        {
            Content = model.Content,
            Rating = model.Rating,
            ReviewDate = DateTime.Now,
            ProductId = product.Id,
            ReviewById = userId,
            IsDigital = isDigital
        };

        await _unitOfWork.Products.AddReview(review);
        await _unitOfWork.Products.SaveAsync();

        return new ApiResponse("Review added to product", StatusEnum.Success, true);
    }

    public async Task<ApiResponse<IEnumerable<LandingProductResponse>>> GetLandPageProducts()
    {
        var responses = await _unitOfWork.Products.GetLandPageProducts().Take(30).ToListAsync();

        return new ApiResponse<IEnumerable<LandingProductResponse>> { Message = "Successful", Status = true, Data = responses, StatusCode = StatusEnum.Success };
    }

    public async Task<ApiResponse<AgentProductProfileResponse>> GetAgentProductProfile(int agentId)
    {
        var responses = await _unitOfWork.Products.GetAgentProductProfile(agentId);

        return new ApiResponse<AgentProductProfileResponse> { Message = "Successful", Status = true, Data = responses, StatusCode = StatusEnum.Success };
    }
    public async Task<ApiResponse<IEnumerable<ProductDetailsResponse>>> GetAgentProducts(AgentProductListRequest request)
    {
        var query = _unitOfWork.Products.GetAgentProducts(request.CategoryId, request.AgentId);

        if (!string.IsNullOrWhiteSpace(request.SearchText))
        {
            query = query
                .Where(s => s.Caption != null && s.Caption.ToLower().Contains(request.SearchText.ToLower()) || s.Name != null && s.Name.ToLower().Contains(request.SearchText.ToLower()));
        }
        var totalProducts = query.Count();
        var paginatedProducts = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize).ToListAsync();


        return new ApiResponse<IEnumerable<ProductDetailsResponse>> { Message = "Successful", Status = true, Data = paginatedProducts, StatusCode = StatusEnum.Success, TotalRecord = totalProducts, Pages = request.PageSize, CurrentPageCount = request.PageNumber };
    }

    public async Task<ApiResponse<IEnumerable<AgentsProfileResponse>>> GetPhysicalAgent(AgentsProfileListRequest request)
    {
        var query = _unitOfWork.Products.GetPhysicalAgents(request);

        var totalProducts = query.Count();
        var paginatedAgents = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize).ToListAsync();

        return new ApiResponse<IEnumerable<AgentsProfileResponse>> { Message = "Successful", Status = true, Data = paginatedAgents, StatusCode = StatusEnum.Success, TotalRecord = totalProducts, Pages = request.PageSize, CurrentPageCount = request.PageNumber };
    }

    public async Task<ApiResponse<ProductDetailResponse>> GetAgentProduct(int productId)
    {
        var responses = await _unitOfWork.Products.GetAgentProduct(productId);

        return new ApiResponse<ProductDetailResponse> { Message = "Successful", Status = true, Data = responses, StatusCode = StatusEnum.Success };
    }
}
