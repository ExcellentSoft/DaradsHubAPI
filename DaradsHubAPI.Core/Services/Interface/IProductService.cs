using DaradsHubAPI.Core.Model;
using DaradsHubAPI.Core.Model.Request;
using DaradsHubAPI.Core.Model.Response;
using DaradsHubAPI.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaradsHubAPI.Core.Services.Interface;
public interface IProductService
{
    Task<ApiResponse> AddProduct(AddAgentHubProductRequest model, string email);
    Task<ApiResponse<ProductDetailResponse>> GetAgentProduct(int productId);
    Task<ApiResponse<AgentProductProfileResponse>> GetAgentProductProfile(int agentId);
    Task<ApiResponse<IEnumerable<ProductDetailsResponse>>> GetAgentProducts(AgentProductListRequest request);
    Task<ApiResponse<IEnumerable<IdNameRecord>>> GetHubProducts(string? searchText);
    Task<ApiResponse<IEnumerable<LandingProductResponse>>> GetLandPageProducts();
    Task<ApiResponse> UpdateProduct(UpdateAgentHubProductRequest model, string email);
    Task<ApiResponse> AddPhysicalReview(AddReviewRequestModel model, int userId);
    Task<ApiResponse<IEnumerable<AgentsProfileResponse>>> GetPhysicalAgent(AgentsProfileListRequest request);
    Task<ApiResponse> CreateProductRequest(CreateHubProductRequest model, string email);
    Task<ApiResponse> AddAgentReview(AddAgentReviewRequest model, int userId);
    Task<ApiResponse> AddDigitalReview(AddReviewRequestModel model, int userId);
    Task<ApiResponse<AgentReviewResponse>> GetAgentReviews(int agentId);
    Task<ApiResponse<ProductReviewResponse>> GetProductReviews(int productId);
    Task<ApiResponse<IEnumerable<HubFAQResponse>>> GetFAQs(string? searchText);
    Task<ApiResponse<IEnumerable<LandingProductResponse>>> GetPublicLandPageProducts();
    Task<ApiResponse<AgentReviewResponse>> GetPublicAgentReviews(int agentId);
    Task<ApiResponse<IEnumerable<AgentsProfileResponse>>> GetPhysicalPublicAgent(AgentsProfileListRequest request);
    Task<ApiResponse<IEnumerable<ProductDetailsResponse>>> GetPublicAgentProducts(AgentProductListRequest request);
    Task<ApiResponse<ProductMetricResponse>> GetProductMetrics(int agentId);
    Task<ApiResponse> DeleteProduct(int productId, bool isDigitalProduct, int agentId);
    Task<ApiResponse<IEnumerable<AgentProductsResponse>>> GetProducts(AgentProductsRequest request, int agentId);
    Task<ApiResponse<ProductOrderMetricResponse>> GetProductOrderMetrics(long productId, bool isDigital);
    Task<ApiResponse<List<AgentOrderListResponse>>> GetProductOrders(ProductOrderListRequest request);
    Task<ApiResponse<IEnumerable<CustomerRequestResponse>>> GetCustomerRequests(CustomerRequestsRequest request, int agentId);
    Task<ApiResponse<SingleCustomerRequestResponse>> GetCustomerRequest(long requestId);
    Task<ApiResponse> ChangeRequestStatus(ChangeRequestStatus request);
    Task<ApiResponse<IEnumerable<AgentReview>>> GetAgentReviews(AgentReviewRequest request, int agentId);
    Task<ApiResponse<AgentHubProductResponse>> GetPhysicalProduct(long productId);
}
