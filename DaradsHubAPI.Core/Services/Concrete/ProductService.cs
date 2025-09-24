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
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
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

    public async Task<ApiResponse<IEnumerable<CustomerRequestResponse>>> GetCustomerRequests(CustomerRequestsRequest request, int agentId)
    {
        var customerRequests = await _unitOfWork.Products.GetCustomerRequests(request, agentId);

        return new ApiResponse<IEnumerable<CustomerRequestResponse>> { Data = customerRequests, Message = "Successful", Status = true, StatusCode = StatusEnum.Success };
    }

    public async Task<ApiResponse<SingleCustomerRequestResponse>> GetCustomerRequest(long requestId)
    {
        var customerRequests = await _unitOfWork.Products.GetCustomerRequest(requestId);
        if (customerRequests is null)
        {
            return new ApiResponse<SingleCustomerRequestResponse> { Message = "Request record not found.", Status = false, StatusCode = StatusEnum.NoRecordFound };
        }
        return new ApiResponse<SingleCustomerRequestResponse> { Data = customerRequests, Message = "Successful", Status = true, StatusCode = StatusEnum.Success };
    }

    public async Task<ApiResponse> ChangeRequestStatus(ChangeRequestStatus request)
    {
        var req = await _unitOfWork.Products.GetHubProductRequest(request.RequestId);
        if (req is null)
        {
            return new ApiResponse("Request record not found.", StatusEnum.NoRecordFound, false);
        }
        var customer = await _unitOfWork.Users.GetSingleWhereAsync(e => e.id == req.CustomerId) ?? new userstb();
        var newNotification = new HubNotification
        {
            TimeCreated = GetLocalDateTime.CurrentDateTime(),
            Title = "Change Request Status",
            NoteToEmail = customer.email,
            Message = $"Your {req.CustomerNeed} has been {request.Status.GetDescription()}",
            NotificationType = NotificationType.ChangeOrderStatus
        };

        req.Status = request.Status;
        await _unitOfWork.Notifications.Insert(newNotification);

        return new ApiResponse("Success.", StatusEnum.Success, true);
    }

    public async Task<ApiResponse> CreateProductRequest(CreateHubProductRequest model, string email)
    {
        var user = await _unitOfWork.Users.GetSingleWhereAsync(d => d.email == email);
        if (model.AgentId < 1)
        {
            return new ApiResponse("Agent is required", StatusEnum.Validation, false);
        }
        var request = new HubProductRequest
        {
            AgentId = model.AgentId,
            Budget = model.Budget,
            DateCreated = DateTime.Now,
            Description = model.Description,
            CustomerNeed = model.CustomerNeed,
            Location = model.Location,
            IsUrgent = model.IsUrgent,
            Status = RequestStatus.Pending,
            ProductRequestTypeEnum = model.ProductRequestTypeEnum,
            PreferredDate = model.PreferredDate,
            Quantity = model.Quantity,
            CategoryId = model.CategoryId,
            CustomerId = user!.id,

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
            Stock = model.Stock,
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

        if (model.Images is not null)
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
        product.Stock = model.Stock;
        product.IsFreeShipping = model.IsFreeShipping;
        product.DateUpdated = DateTime.Now;
        product.Caption = model.Caption;
        product.EstimateDeliveryTime = model.EstimateDeliveryTime;

        if (model.Images is not null)
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

    public async Task<ApiResponse> AddPhysicalReview(AddReviewRequestModel model, int userId)
    {
        var product = await _unitOfWork.Products.GetSingleWhereAsync(x => x.Id == model.productId);
        if (product == null)
            return new ApiResponse($"Product record not found", StatusEnum.NoRecordFound, false);

        var review = new HubReview
        {
            Content = model.Content,
            Rating = model.Rating,
            ReviewDate = GetLocalDateTime.CurrentDateTime(),
            ProductId = product.Id,
            ReviewById = userId,
            IsDigital = false
        };

        await _unitOfWork.Products.AddReview(review);
        await _unitOfWork.Products.SaveAsync();

        return new ApiResponse("Review added to product", StatusEnum.Success, true);
    }

    public async Task<ApiResponse> AddDigitalReview(AddReviewRequestModel model, int userId)
    {
        var product = await _unitOfWork.DigitalProducts.GetSingleWhereAsync(x => x.Id == model.productId);
        if (product == null)
            return new ApiResponse($"Product record not found", StatusEnum.NoRecordFound, false);

        var review = new HubReview
        {
            Content = model.Content,
            Rating = model.Rating,
            ReviewDate = GetLocalDateTime.CurrentDateTime(),
            ProductId = product.Id,
            ReviewById = userId,
            IsDigital = true
        };
        await _unitOfWork.Products.AddReview(review);
        await _unitOfWork.Products.SaveAsync();
        return new ApiResponse("Review added to product", StatusEnum.Success, true);
    }

    public async Task<ApiResponse> AddAgentReview(AddAgentReviewRequest model, int userId)
    {
        var agent = await _unitOfWork.Users.GetSingleWhereAsync(x => x.id == model.AgentId);
        if (agent == null)
            return new ApiResponse($"Agent record not found", StatusEnum.NoRecordFound, false);

        var review = new HubAgentReview
        {
            Content = model.Content,
            Rating = model.Rating,
            ReviewDate = GetLocalDateTime.CurrentDateTime(),
            AgentId = agent.id,
            ReviewById = userId,
        };

        await _unitOfWork.Users.AddAgentReview(review);

        return new ApiResponse("Review added to an agent", StatusEnum.Success, true);
    }

    public async Task<ApiResponse<AgentReviewResponse>> GetAgentReviews(int agentId)
    {
        var review = await _unitOfWork.Products.GetReviewByAgentId(agentId);

        return new ApiResponse<AgentReviewResponse> { Data = review, Message = "Successful", Status = true, StatusCode = StatusEnum.Success };
    }

    public async Task<ApiResponse<AgentHubProductResponse>> GetPhysicalProduct(long productId)
    {
        var product = await _unitOfWork.Products.GetSingleWhereAsync(e => e.Id == productId);
        if (product is null)
        {
            return new ApiResponse<AgentHubProductResponse> { Message = "Product record not found", Status = false, StatusCode = StatusEnum.NoRecordFound };
        }
        var response = new AgentHubProductResponse
        {
            Caption = product.Caption,
            CategoryId = product.CategoryId,
            DeliveryPrice = product.DeliveryPrice,
            Description = product.Description,
            DiscountPrice = product.DiscountPrice,
            EstimateDeliveryTime = product.EstimateDeliveryTime,
            IsFreeShipping = product.IsFreeShipping,
            Price = product.Price,
            ProductId = product.ProductId,
            SKU = product.SKU,
            Stock = product.Stock,
            SubCategoryId = product.SubCategoryId,
        };
        response.Images = await _unitOfWork.Products.GetPhysicalProductImages(productId);
        return new ApiResponse<AgentHubProductResponse> { Data = response, Message = "Successful", Status = true, StatusCode = StatusEnum.Success };
    }

    public async Task<ApiResponse<AgentReviewResponse>> GetPublicAgentReviews(int agentId)
    {
        var review = await _unitOfWork.Products.GetReviewByPubicAgentId(agentId);

        return new ApiResponse<AgentReviewResponse> { Data = review, Message = "Successful", Status = true, StatusCode = StatusEnum.Success };
    }

    public async Task<ApiResponse<ProductReviewResponse>> GetProductReviews(int productId)
    {
        var review = await _unitOfWork.Products.GetReviewByProductId(productId);

        return new ApiResponse<ProductReviewResponse> { Data = review, Message = "Successful", Status = true, StatusCode = StatusEnum.Success };
    }

    public async Task<ApiResponse<IEnumerable<LandingProductResponse>>> GetLandPageProducts()
    {
        var responses = await _unitOfWork.Products.GetLandPageProducts().Take(30).ToListAsync();

        return new ApiResponse<IEnumerable<LandingProductResponse>> { Message = "Successful", Status = true, Data = responses, StatusCode = StatusEnum.Success };
    }

    public async Task<ApiResponse<IEnumerable<LandingProductResponse>>> GetPublicLandPageProducts()
    {
        var responses = await _unitOfWork.Products.GetPublicLandPageProducts().Take(30).ToListAsync();

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

    public async Task<ApiResponse<IEnumerable<AgentReview>>> GetAgentReviews(AgentReviewRequest request, int agentId)
    {
        var query = _unitOfWork.Products.GetAgentReviews(request, agentId);

        var totalProducts = query.Count();
        var paginatedReviews = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize).ToListAsync();

        return new ApiResponse<IEnumerable<AgentReview>> { Message = "Successful", Status = true, Data = paginatedReviews, StatusCode = StatusEnum.Success, TotalRecord = totalProducts, Pages = request.PageSize, CurrentPageCount = request.PageNumber };
    }

    public async Task<ApiResponse<IEnumerable<ProductDetailsResponse>>> GetPublicAgentProducts(AgentProductListRequest request)
    {
        var query = _unitOfWork.Products.GetPublicAgentProducts(request.CategoryId, request.AgentId);

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

    public async Task<ApiResponse<IEnumerable<HubFAQResponse>>> GetFAQs(string? searchText)
    {
        var query = _unitOfWork.Products.GetFAQs();

        if (!string.IsNullOrWhiteSpace(searchText))
        {
            query = query
                .Where(s => s.Question.ToLower().Contains(searchText.ToLower()) || s.Answer.ToLower().Contains(searchText.ToLower()));
        }
        var faq = await query.ToListAsync();

        return new ApiResponse<IEnumerable<HubFAQResponse>> { Message = "Successful", Status = true, Data = faq, StatusCode = StatusEnum.Success };
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

    public async Task<ApiResponse<IEnumerable<AgentsProfileResponse>>> GetPhysicalPublicAgent(AgentsProfileListRequest request)
    {
        var query = _unitOfWork.Products.GetPhysicalPublicAgents(request);

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

    public async Task<ApiResponse<ProductMetricResponse>> GetProductMetrics(int agentId)
    {
        var responses = await _unitOfWork.Products.GetProductMetrics(agentId);
        return new ApiResponse<ProductMetricResponse> { Message = "Successful", Status = true, Data = responses, StatusCode = StatusEnum.Success };
    }

    public async Task<ApiResponse> DeleteProduct(int productId, bool isDigitalProduct, int agentId)
    {
        await _unitOfWork.Products.DeleteProduct(productId, isDigitalProduct, agentId);
        return new ApiResponse("Successful", StatusEnum.Success, false);
    }

    public async Task<ApiResponse<IEnumerable<AgentProductsResponse>>> GetProducts(AgentProductsRequest request, int agentId)
    {
        var digitalProducts = await _unitOfWork.Products.GetDigiatlProducts(request, agentId).ToListAsync();
        var physicalProducts = await _unitOfWork.Products.GetPhysicalProducts(request, agentId).ToListAsync();

        var finalData = digitalProducts.Concat(physicalProducts);
        var totalProducts = finalData.Count();
        var paginatedProducts = finalData
            .Skip((request.PageNumber - 1) * request.PageSize)
        .Take(request.PageSize).ToList();
        return new ApiResponse<IEnumerable<AgentProductsResponse>> { Message = "Successful", Status = true, Data = paginatedProducts, StatusCode = StatusEnum.Success, TotalRecord = totalProducts, Pages = request.PageSize, CurrentPageCount = request.PageNumber };
    }

    public async Task<ApiResponse<IEnumerable<IdNameRecord>>> GetAgentCategories(string? searchText, int agentId)
    {
        var products = _unitOfWork.Products.GetAgentCategories(searchText, agentId);
        var iProducts = await products.Select(c => new IdNameRecord
        {
            Id = c.id,
            Name = c.name
        }).ToListAsync();
        return new ApiResponse<IEnumerable<IdNameRecord>> { Data = iProducts, Message = "Successful", Status = true, StatusCode = StatusEnum.Success };
    }

    public async Task<ApiResponse<ProductOrderMetricResponse>> GetProductOrderMetrics(long productId, bool isDigital)
    {
        var responses = new ProductOrderMetricResponse();
        if (isDigital)
        {
            responses = await _unitOfWork.Products.GetDigitalProductOrderMetrics(productId);
        }
        else
        {
            responses = await _unitOfWork.Products.GetPhysicalProductOrderMetrics(productId);
        }
        return new ApiResponse<ProductOrderMetricResponse> { Message = "Successful", Status = true, Data = responses, StatusCode = StatusEnum.Success };
    }

    public async Task<ApiResponse<CustomerRequestMetricResponse>> GetCustomerRequestMetrics(long agentId)
    {
        var responses = await _unitOfWork.Products.GetCustomerRequestMetrics(agentId);
        return new ApiResponse<CustomerRequestMetricResponse>
        {
            Message = "Successful",
            Status = true,
            Data = responses,
            StatusCode = StatusEnum.Success

        };
    }

    public async Task<ApiResponse<List<AgentOrderListResponse>>> GetProductOrders(ProductOrderListRequest request)
    {
        var response = new List<AgentOrderListResponse>();
        if (request.IsDigital)
        {
            response = await _unitOfWork.Products.GetDigitalProductOrders(request);
        }
        else
        {
            response = await _unitOfWork.Products.GetPhysicalProductOrders(request);
        }

        var totalRecordsCount = response.Count;

        return new ApiResponse<List<AgentOrderListResponse>> { StatusCode = StatusEnum.Success, Message = "Products Orders fetched successfully.", Status = true, Data = response, Pages = request.PageSize, TotalRecord = totalRecordsCount, CurrentPage = request.PageNumber };
    }
}