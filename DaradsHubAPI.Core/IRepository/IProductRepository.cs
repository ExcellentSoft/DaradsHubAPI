using DaradsHubAPI.Core.Model.Request;
using DaradsHubAPI.Core.Model.Response;
using DaradsHubAPI.Domain.Entities;

namespace DaradsHubAPI.Core.IRepository;
public interface IProductRepository : IGenericRepository<HubAgentProduct>
{
    Task AddHubProductImages(ProductImages productImages);
    Task AddHubProductRequestImages(ProductRequestImages productImages);
    Task AddReview(HubReview review);
    Task CreateHubProductRequest(HubProductRequest request);
    Task DeleteProduct(long productId, bool isDigital, int agentId);
    Task<ProductDetailResponse> GetAgentProduct(long productId);
    Task<AgentProductProfileResponse> GetAgentProductProfile(int agentId);
    IQueryable<ProductDetailsResponse> GetAgentProducts(int categoryId, int agentId);
    Task<SingleCustomerRequestResponse?> GetCustomerRequest(long requestId);
    Task<List<CustomerRequestResponse>> GetCustomerRequests(CustomerRequestsRequest request, int agentId);
    Task<DashboardMetricResponse> GetDashboardMetrics(int agentId, string userEmail);
    Task<ProductOrderMetricResponse> GetDigitalProductOrderMetrics(long productId);
    Task<List<AgentOrderListResponse>> GetDigitalProductOrders(ProductOrderListRequest request);
    IQueryable<HubFAQResponse> GetFAQs();
    Task<HubProductRequest?> GetHubProductRequest(long id);
    IQueryable<HubProduct> GetHubProducts(string? searchText);
    IQueryable<LandingProductResponse> GetLandPageProducts();
    Task<IEnumerable<AgentReview>> GetLatestReview(int agentId);
    IQueryable<AgentsProfileResponse> GetPhysicalAgents(AgentsProfileListRequest request);
    Task<ProductOrderMetricResponse> GetPhysicalProductOrderMetrics(long productId);
    Task<List<AgentOrderListResponse>> GetPhysicalProductOrders(ProductOrderListRequest request);
    IQueryable<AgentsProfileResponse> GetPhysicalPublicAgents(AgentsProfileListRequest request);
    Task<HubAgentProduct?> GetProduct(long productId);
    Task<ProductMetricResponse> GetProductMetrics(int agentId);
    IQueryable<AgentProductsResponse> GetProducts(AgentProductsRequest request, int agentId);
    IQueryable<ProductDetailsResponse> GetPublicAgentProducts(int categoryId, int agentId);
    IQueryable<LandingProductResponse> GetPublicLandPageProducts();
    Task<AgentReviewResponse> GetReviewByAgentId(int agentId);
    Task<ProductReviewResponse> GetReviewByProductId(int productId);
    Task<AgentReviewResponse> GetReviewByPubicAgentId(int agentId);
    IQueryable<AgentReview> GetAgentReviews(AgentReviewRequest request, int agentId);
    IQueryable<AgentProductsResponse> GetDigiatlProducts(AgentProductsRequest request, int agentId);
    IQueryable<AgentProductsResponse> GetPhysicalProducts(AgentProductsRequest request, int agentId);
    Task<IEnumerable<string>> GetPhysicalProductImages(long productId);
    Task<List<CustomerRequestResponse>> GetCustomerRequestsForAdmin(CustomerRequestsRequest request);
    Task<CustomerRequestMetricResponse> GetCustomerRequestMetrics(long agentId);
    Task<CustomerRequestMetricResponse> GetCustomerRequestMetricsForAdmin();
    IQueryable<category> GetAgentCategories(string? searchText, int agentId);
    Task<List<AgentOrderListResponse>> GetAgentProductOrders(AgentProductOrderListRequest request);
    IQueryable<SimilarProductResponse> GetSimilarProducts(long productId);
}
