using DaradsHubAPI.Core.Model.Response;
using DaradsHubAPI.Domain.Entities;

namespace DaradsHubAPI.Core.IRepository;
public interface IDigitalProductRepository : IGenericRepository<HubDigitalProduct>
{
    Task AddHubDigitalProductImages(DigitalProductImages productImages);
    Task<DigitalProductDetailResponse> GetAgentDigitalProduct(long productId);
    Task<AgentProductProfileResponse> GetAgentDigitalProductProfile(int agentId);
    IQueryable<DigitalProductDetailsResponse> GetAgentDigitalProducts(int catalogueId, int agentId);
    IQueryable<AgentsProfileResponse> GetDigitalAgents(AgentsProfileListRequest request);
    IQueryable<Catalogue> GetDigitalProducts(string? searchText, int agentId);
    IQueryable<LandingPageDigitalProductResponse> GetLandPageProducts();
}
