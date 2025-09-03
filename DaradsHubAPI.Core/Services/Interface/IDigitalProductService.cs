using DaradsHubAPI.Core.Model;
using DaradsHubAPI.Core.Model.Request;
using DaradsHubAPI.Core.Model.Response;
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
    Task<ApiResponse<IEnumerable<IdNameRecord>>> GetDigitalProducts(string? searchText, int agentId);
    Task<ApiResponse<IEnumerable<LandingPageDigitalProductResponse>>> GetLandPageProducts();
    Task<ApiResponse> UpdateDigitalProduct(UpdateDigitalHubProductRequest model, string email);
}
