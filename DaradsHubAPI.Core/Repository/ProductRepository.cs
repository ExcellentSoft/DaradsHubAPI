using DaradsHubAPI.Core.IRepository;
using DaradsHubAPI.Core.Model.Response;
using DaradsHubAPI.Domain.Entities;
using DaradsHubAPI.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace DaradsHubAPI.Core.Repository;
public class ProductRepository(AppDbContext _context) : GenericRepository<HubAgentProduct>(_context), IProductRepository
{
    #region Physical Product    
    public IQueryable<HubProduct> GetHubProducts(string? searchText)
    {
        searchText = searchText?.Trim().ToLower();
        var hubProducts = _context.HubProducts.Where(s => searchText == null || s.Name.ToLower().Contains(searchText));
        return hubProducts;
    }
    public async Task AddHubProductImages(ProductImages productImages)
    {
        _context.ProductImages.Add(productImages);
        await Task.CompletedTask;
    }
    public async Task AddReview(HubReview review)
    {
        _context.HubReviews.Add(review);
        await Task.CompletedTask;
    }
    public IQueryable<LandingProductResponse> GetLandPageProducts()
    {
        var query = (from ph in _context.HubAgentProducts
                     join img in _context.ProductImages on ph.Id equals img.ProductId
                     orderby ph.DateCreated descending
                     select new { ph, img }).GroupBy(d => d.img.ProductId).Select(f => new LandingProductResponse
                     {
                         Id = f.Key,
                         Caption = f.Select(e => e.ph.Caption).FirstOrDefault(),
                         AgentId = f.Select(e => e.ph.AgentId).FirstOrDefault(),
                         Description = f.Select(e => e.ph.Description).FirstOrDefault(),
                         ImageUrl = f.Select(e => e.img.ImageUrl).FirstOrDefault()
                     });
        return query;
    }
    public async Task<AgentProductProfileResponse> GetAgentProductProfile(int agentId)
    {

        var response = new AgentProductProfileResponse();
        var user = await _context.userstb.FirstOrDefaultAsync(d => d.id == agentId && d.IsAgent == true);
        if (user != null)
        {
            response.BusinessName = user.BusinessName;
            response.FullName = user.fullname;
            response.IsOnline = true;
            response.IsVerify = true;
            response.AgentId = user.id;
            response.Photo = user.Photo;

            var query = from hp in _context.HubAgentProducts.Where(s => s.AgentId == agentId)
                        join p in _context.HubProducts on hp.ProductId equals p.Id
                        select new { hp, p };

            var rQuery = from hp in _context.HubAgentProducts.Where(s => s.AgentId == agentId)
                         join r in _context.HubReviews on hp.Id equals r.ProductId
                         where r.IsDigital == false
                         select new { r };

            response.SellingProducts = await query.Select(d => d.p.Name).Distinct().Take(10).ToListAsync();
            response.ReviewCount = rQuery.Select(r => r.r.ProductId).Count();
            response.MaxRating = rQuery.Sum(r => r.r.Rating) / 100;

            var address = await _context.ShippingAddresses.Where(r => r.CustomerId == agentId).FirstOrDefaultAsync();
            if (address is not null)
            {
                response.State = address.State;
                response.City = address.City;
                response.Address = address.Address;
            }
            var profile = await _context.HubAgentProfiles.Where(r => r.UserId == agentId).FirstOrDefaultAsync();
            if (profile is not null)
            {
                response.Experience = profile.Experience;
            }

            return response;
        }
        return response;
    }

    public IQueryable<AgentsProfileResponse> GetPhysicalAgents(AgentsProfileListRequest request)
    {
        var uquery = from user in _context.userstb.Where(d => d.IsAgent == true)

                     select new AgentsProfileResponse
                     {
                         BusinessName = user.BusinessName,
                         FullName = user.fullname,
                         IsOnline = true,
                         IsVerify = true,
                         AgentId = user.id,
                         Photo = user.Photo,
                         SellingProducts = (from hp in _context.HubAgentProducts.Where(s => s.AgentId == user.id)
                                            join p in _context.HubProducts on hp.ProductId equals p.Id
                                            select p).Select(d => d.Name).Distinct().Take(10).ToList(),
                         ReviewCount = (from hp in _context.HubAgentProducts.Where(s => s.AgentId == user.id)
                                        join r in _context.HubReviews on hp.Id equals r.ProductId
                                        where r.IsDigital == false
                                        select r).Select(r => r.ProductId).Count(),
                         MaxRating = (from hp in _context.HubAgentProducts.Where(s => s.AgentId == user.id)
                                      join r in _context.HubReviews on hp.Id equals r.ProductId
                                      where r.IsDigital == false
                                      select r).Sum(r => r.Rating) / 100,
                         Experience = _context.HubAgentProfiles.Where(r => r.UserId == user.id).Select(e => e.Experience).FirstOrDefault(),
                         AgentsAddress = _context.ShippingAddresses.Where(r => r.CustomerId == user.id).Select(n => new AgentsAddress
                         {
                             Address = n.Address,
                             City = n.City,
                             State = n.State
                         }).FirstOrDefault()
                     };

        if (request.IsOnline is not null)
        {
            uquery = uquery.Where(e => e.IsOnline == request.IsOnline);
        }
        if (!string.IsNullOrWhiteSpace(request.ProductName))
        {
            uquery = uquery.Where(e => e.SellingProducts.Contains(request.ProductName));
        }
        if (!string.IsNullOrWhiteSpace(request.Location))
        {
            uquery = uquery.Where(e => e.AgentsAddress != null && e.AgentsAddress.Address != null && e.AgentsAddress.Address.Contains(request.Location) || (e.AgentsAddress!.City != null && e.AgentsAddress.City.Contains(request.Location)) || (e.AgentsAddress!.State != null && e.AgentsAddress.State.Contains(request.Location)));
        }

        return uquery;
    }

    public IQueryable<ProductDetailsResponse> GetAgentProducts(int categoryId, int agentId)
    {
        var query = (from ph in _context.HubAgentProducts.Where(d => d.AgentId == agentId)
                     join img in _context.ProductImages on ph.Id equals img.ProductId
                     join p in _context.HubProducts on ph.ProductId equals p.Id
                     where ph.CategoryId == categoryId || categoryId == 0
                     orderby ph.DateCreated descending
                     select new { ph, img, p }).GroupBy(d => d.img.ProductId).Select(f => new ProductDetailsResponse
                     {
                         ProductId = f.Key,
                         Caption = f.Select(e => e.ph.Caption).FirstOrDefault(),
                         Name = f.Select(e => e.p.Name).FirstOrDefault(),
                         Price = f.Select(e => e.ph.Price).FirstOrDefault(),
                         Description = f.Select(e => e.ph.Description).FirstOrDefault(),
                         ImageUrl = f.Select(e => e.img.ImageUrl).FirstOrDefault(),
                         ReviewCount = _context.HubReviews.Where(d => d.IsDigital == false && d.ProductId == f.Key).Count(),
                         MaxRating = _context.HubReviews.Where(d => d.IsDigital == false && d.ProductId == f.Key).Sum(d => d.Rating) / 100
                     });
        return query;
    }
    public async Task<ProductDetailResponse> GetAgentProduct(long productId)
    {
        var query = await (from ph in _context.HubAgentProducts.Where(d => d.Id == productId)
                           join user in _context.userstb on ph.AgentId equals user.id
                           join img in _context.ProductImages on ph.Id equals img.ProductId
                           join p in _context.HubProducts on ph.ProductId equals p.Id
                           select new { ph, img, p, user }).GroupBy(d => d.img.ProductId).Select(f => new ProductDetailResponse
                           {
                               ProductId = f.Key,
                               Caption = f.Select(e => e.ph.Caption).FirstOrDefault(),
                               Name = f.Select(e => e.p.Name).FirstOrDefault(),
                               AgentName = f.Select(e => e.user.BusinessName).FirstOrDefault(),
                               Price = f.Select(e => e.ph.Price).FirstOrDefault(),
                               Description = f.Select(e => e.ph.Description).FirstOrDefault(),
                               ImageUrl = f.Select(e => e.img.ImageUrl).ToList(),
                               ReviewCount = _context.HubReviews.Where(d => d.IsDigital == false && d.ProductId == f.Key).Count(),
                               MaxRating = _context.HubReviews.Where(d => d.IsDigital == false && d.ProductId == f.Key).Sum(d => d.Rating) / 100
                           }).FirstOrDefaultAsync();
        return query ?? new ProductDetailResponse { };
    }
    #endregion
}