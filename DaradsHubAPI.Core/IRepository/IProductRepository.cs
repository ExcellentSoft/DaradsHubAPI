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
    Task<ProductDetailResponse> GetAgentProduct(long productId);
    Task<AgentProductProfileResponse> GetAgentProductProfile(int agentId);
    IQueryable<ProductDetailsResponse> GetAgentProducts(int categoryId, int agentId);
    IQueryable<HubFAQResponse> GetFAQs();
    IQueryable<HubProduct> GetHubProducts(string? searchText);
    IQueryable<LandingProductResponse> GetLandPageProducts();
    IQueryable<AgentsProfileResponse> GetPhysicalAgents(AgentsProfileListRequest request);
    Task<HubAgentProduct?> GetProduct(long productId);
    Task<AgentReviewResponse> GetReviewByAgentId(int agentId);
    Task<ProductReviewResponse> GetReviewByProductId(int productId);
}
