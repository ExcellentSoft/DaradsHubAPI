using DaradsHubAPI.Core.Model.Response;
using DaradsHubAPI.Domain.Entities;

namespace DaradsHubAPI.Core.IRepository;
public interface IProductRepository : IGenericRepository<HubAgentProduct>
{
    Task AddHubProductImages(ProductImages productImages);
    Task AddReview(HubReview review);
    Task<ProductDetailResponse> GetAgentProduct(long productId);
    Task<AgentProductProfileResponse> GetAgentProductProfile(int agentId);
    IQueryable<ProductDetailsResponse> GetAgentProducts(int categoryId, int agentId);
    IQueryable<HubProduct> GetHubProducts(string? searchText);
    IQueryable<LandingProductResponse> GetLandPageProducts();
}
