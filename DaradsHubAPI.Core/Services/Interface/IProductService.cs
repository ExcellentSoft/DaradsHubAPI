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
    Task<ApiResponse> AddReview(AddReviewRequestModel model, int userId, bool isDigital);
    Task<ApiResponse<IEnumerable<AgentsProfileResponse>>> GetPhysicalAgent(AgentsProfileListRequest request);
    Task<ApiResponse> CreateProductRequest(CreateHubProductRequest model, string email);
}
