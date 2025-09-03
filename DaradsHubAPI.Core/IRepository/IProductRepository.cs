using DaradsHubAPI.Core.Model.Response;
using DaradsHubAPI.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaradsHubAPI.Core.IRepository;
public interface IProductRepository : IGenericRepository<HubAgentProduct>
{
    Task AddHubProductImages(ProductImages productImages);
    Task AddReview(Review review);
    Task<ProductDetailResponse> GetAgentProduct(long productId);
    Task<AgentProductProfileResponse> GetAgentProductProfile(int agentId);
    IQueryable<ProductDetailsResponse> GetAgentProducts(int categoryId, int agentId);
    IQueryable<HubProduct> GetHubProducts(string? searchText);
    IQueryable<LandingProductProductResponse> GetLandPageProducts();
}
