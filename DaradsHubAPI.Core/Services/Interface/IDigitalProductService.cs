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
public interface IDigitalProductService
{
    Task<ApiResponse> AddDigitalProduct(AddDigitalHubProductRequest model, string email);
    Task<ApiResponse<AgentProductProfileResponse>> GetAgentDigitalProductProfile(int agentId);
    Task<ApiResponse<DigitalProductDetailResponse>> GetAgentProduct(int productId);
    Task<ApiResponse<IEnumerable<DigitalProductDetailsResponse>>> GetAgentProducts(AgentDigitalProductListRequest request);
    Task<ApiResponse<IEnumerable<AgentsProfileResponse>>> GetDigitalAgents(AgentsProfileListRequest request);
    Task<ApiResponse<DigitalHubProductResponse>> GetDigitalProduct(long productId);
    Task<ApiResponse<IEnumerable<IdNameRecord>>> GetDigitalProducts(string? searchText, int agentId);
    Task<ApiResponse<IEnumerable<AgentsProfileResponse>>> GetDigitalPublicAgents(AgentsProfileListRequest request);
    Task<ApiResponse<IEnumerable<LandingPageDigitalProductResponse>>> GetLandPageProducts();
    Task<ApiResponse<AgentProductProfileResponse>> GetPublicAgentDigitalProductProfile(int agentId);
    Task<ApiResponse<IEnumerable<DigitalProductDetailsResponse>>> GetPublicAgentProducts(AgentDigitalProductListRequest request);
    Task<ApiResponse<IEnumerable<LandingPageDigitalProductResponse>>> GetPublicLandPageProducts();
    Task<ApiResponse<IEnumerable<SimilarProductResponse>>> GetSimilarDigitalProducts(long productId);
    Task<ApiResponse> UpdateDigitalProduct(UpdateDigitalHubProductRequest model, string email);
}
