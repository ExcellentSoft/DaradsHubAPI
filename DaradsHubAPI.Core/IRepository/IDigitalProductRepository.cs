using DaradsHubAPI.Core.Model.Response;
using DaradsHubAPI.Domain.Entities;

namespace DaradsHubAPI.Core.IRepository;
public interface IDigitalProductRepository : IGenericRepository<HubDigitalProduct>
{
    Task AddHubDigitalProductImages(DigitalProductImages productImages);
    Task<DigitalProductDetailResponse> GetAgentDigitalProduct(long productId);
    Task<AgentProductProfileResponse> GetAgentDigitalProductProfile(int agentId);
    IQueryable<DigitalProductDetailsResponse> GetAgentDigitalProducts(int catalogueId, int agentId);
    Task<Catalogue> GetCatalogue(long catalogueId);
    IQueryable<AgentsProfileResponse> GetDigitalAgents(AgentsProfileListRequest request);
    Task<IEnumerable<string>> GetDigitalProductImages(long productId);
    IQueryable<Catalogue> GetDigitalProducts(string? searchText, int agentId);
    IQueryable<AgentsProfileResponse> GetDigitalPublicAgents(AgentsProfileListRequest request);
    IQueryable<LandingPageDigitalProductResponse> GetLandPageProducts();
    Task<AgentProductProfileResponse> GetPublicAgentDigitalProductProfile(int agentId);
    IQueryable<DigitalProductDetailsResponse> GetPublicAgentDigitalProducts(int catalogueId, int agentId);
    IQueryable<LandingPageDigitalProductResponse> GetPublicLandPageProducts();
}
