using DaradsHubAPI.Core.IRepository;
using DaradsHubAPI.Core.Model.Response;
using DaradsHubAPI.Domain.Entities;
using DaradsHubAPI.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace DaradsHubAPI.Core.Repository;
public class DigitalProductRepository(AppDbContext _context) : GenericRepository<HubDigitalProduct>(_context), IDigitalProductRepository
{
    public IQueryable<Catalogue> GetDigitalProducts(string? searchText, int agentId)
    {
        searchText = searchText?.Trim().ToLower();

        var query = from mapping in _context.CatalogueMappings
                    where mapping.AgentId == agentId
                    join catalogue in _context.Catalogues on mapping.CatalogueId equals catalogue.Id
                    select catalogue;

        if (!string.IsNullOrEmpty(searchText))
        {
            query = query.Where(d => d.Name.ToLower().Contains(searchText));
        }
        return query;
    }

    public async Task<Catalogue> GetCatalogue(long catalogueId)
    {
        var catalogue = await _context.Catalogues.Where(r => r.Id == catalogueId).FirstOrDefaultAsync();

        return catalogue ?? new Catalogue();
    }

    public async Task AddHubDigitalProductImages(DigitalProductImages productImages)
    {
        _context.DigitalProductImages.Add(productImages);
        await SaveAsync();
    }

    public IQueryable<LandingPageDigitalProductResponse> GetLandPageProducts()
    {
        var query = (from ph in _context.HubDigitalProducts
                     join img in _context.DigitalProductImages on ph.Id equals img.ProductId
                     join c in _context.Catalogues on ph.CatalogueId equals c.Id
                     orderby ph.DateCreated descending
                     select new { ph, img, c }).GroupBy(d => d.img.ProductId).Select(f => new LandingPageDigitalProductResponse
                     {
                         Id = f.Key,
                         AgentId = f.Select(e => e.ph.AgentId).FirstOrDefault(),
                         Description = f.Select(e => e.ph.Description).FirstOrDefault(),
                         ImageUrls = f.Select(e => e.img.ImageUrl).ToList(),
                         Title = f.Select(e => e.c.Name).FirstOrDefault(),
                     });
        return query;
    }
    public async Task<IEnumerable<string>> GetDigitalProductImages(long productId)
    {
        var images = await _context.DigitalProductImages.Where(s => s.ProductId == productId).Select(d => d.ImageUrl).ToListAsync();
        return images;
    }


    public IQueryable<LandingPageDigitalProductResponse> GetPublicLandPageProducts()
    {
        var query = (from user in _context.userstb
                     where user.IsAgent == true && user.IsPublicAgent == true
                     join ph in _context.HubDigitalProducts on user.id equals ph.AgentId
                     join img in _context.DigitalProductImages on ph.Id equals img.ProductId
                     join c in _context.Catalogues on ph.CatalogueId equals c.Id
                     orderby ph.DateCreated descending
                     select new { ph, img, c }).GroupBy(d => d.img.ProductId).Select(f => new LandingPageDigitalProductResponse
                     {
                         Id = f.Key,
                         AgentId = f.Select(e => e.ph.AgentId).FirstOrDefault(),
                         Description = f.Select(e => e.ph.Description).FirstOrDefault(),
                         ImageUrls = f.Select(e => e.img.ImageUrl).ToList(),
                         Title = f.Select(e => e.c.Name).FirstOrDefault(),
                     });
        return query;
    }

    public async Task<AgentProductProfileResponse> GetAgentDigitalProductProfile(int agentId)
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

            var query = from hp in _context.HubDigitalProducts.Where(s => s.AgentId == agentId)
                        join p in _context.Catalogues on hp.CatalogueId equals p.Id
                        select new { p };
            var dquery = from hp in _context.HubAgentProducts.Where(s => s.AgentId == agentId)
                         join p in _context.categories on hp.CategoryId equals p.id
                         select new { hp, p };


            var rQuery = from hp in _context.HubDigitalProducts.Where(s => s.AgentId == agentId)
                         join r in _context.HubReviews on hp.Id equals r.ProductId
                         where r.IsDigital == true
                         select new { r };
            response.AnotherSellingProducts = await dquery.Select(d => d.p.name).Distinct().Take(10).ToListAsync();
            response.SellingProducts = await query.Select(d => d.p.Name).Distinct().Take(10).ToListAsync();
            response.ReviewCount = await rQuery.Select(r => r.r).CountAsync();
            response.MaxRating = await rQuery.SumAsync(r => r.r.Rating) / 100;

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
    public async Task<AgentProductProfileResponse> GetPublicAgentDigitalProductProfile(int agentId)
    {

        var response = new AgentProductProfileResponse();
        var user = await _context.userstb.FirstOrDefaultAsync(d => d.id == agentId && d.IsAgent == true && d.IsPublicAgent == true);
        if (user != null)
        {
            response.BusinessName = user.BusinessName;
            response.FullName = user.fullname;
            response.IsOnline = true;
            response.IsVerify = true;
            response.AgentId = user.id;
            response.Photo = user.Photo;

            var query = from hp in _context.HubDigitalProducts.Where(s => s.AgentId == agentId)
                        join p in _context.Catalogues on hp.CatalogueId equals p.Id
                        select new { p };

            var dquery = from hp in _context.HubAgentProducts.Where(s => s.AgentId == agentId)
                         join p in _context.categories on hp.CategoryId equals p.id
                         select new { hp, p };


            var rQuery = from hp in _context.HubDigitalProducts.Where(s => s.AgentId == agentId)
                         join r in _context.HubReviews on hp.Id equals r.ProductId
                         where r.IsDigital == true
                         select new { r };

            response.AnotherSellingProducts = await dquery.Select(d => d.p.name).Distinct().Take(10).ToListAsync();
            response.SellingProducts = await query.Select(d => d.p.Name).Distinct().Take(10).ToListAsync();
            response.ReviewCount = await rQuery.Select(r => r.r).CountAsync();
            response.MaxRating = await rQuery.SumAsync(r => r.r.Rating) / 100;

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

    public IQueryable<AgentsProfileResponse> GetDigitalAgents(AgentsProfileListRequest request)
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
                         SellingProducts = (from hp in _context.HubDigitalProducts.Where(s => s.AgentId == user.id)
                                            join p in _context.Catalogues on hp.CatalogueId equals p.Id
                                            join i in _context.DigitalProductImages on hp.Id equals i.ProductId
                                            select new { p, hp, i }).GroupBy(z => z.p.Name)
                                            .Select(d => new SellingProduct
                                            {
                                                Name = d.Key,
                                                Image = d.Select(e => e.i.ImageUrl).FirstOrDefault()
                                            }).Take(10).ToList(),
                         ReviewCount = (from hp in _context.HubDigitalProducts.Where(s => s.AgentId == user.id)
                                        join r in _context.HubReviews on hp.Id equals r.ProductId
                                        where r.IsDigital == true
                                        select r).Select(r => r.ProductId).Count(),
                         MaxRating = (from hp in _context.HubDigitalProducts.Where(s => s.AgentId == user.id)
                                      join r in _context.HubReviews on hp.Id equals r.ProductId
                                      where r.IsDigital == true
                                      select r.Rating).OrderByDescending(r => r).FirstOrDefault(),
                         Experience = _context.HubAgentProfiles.Where(r => r.UserId == user.id).Select(e => e.Experience).FirstOrDefault(),
                         AgentsAddress = _context.ShippingAddresses.Where(r => r.CustomerId == user.id && (request.Location == null || r.State.Contains(request.Location))).Select(n => new AgentsAddress
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
            uquery = uquery.Where(e => e.SellingProducts.Any(r => r.Name!.Contains(request.ProductName)));
        }
        return uquery;
    }
    public IQueryable<AgentsProfileResponse> GetDigitalPublicAgents(AgentsProfileListRequest request)
    {
        var uquery = from user in _context.userstb.Where(d => d.IsAgent == true && d.IsPublicAgent == true)
                     select new AgentsProfileResponse
                     {
                         BusinessName = user.BusinessName,
                         FullName = user.fullname,
                         IsOnline = true,
                         IsVerify = true,
                         AgentId = user.id,
                         Photo = user.Photo,
                         SellingProducts = (from hp in _context.HubDigitalProducts.Where(s => s.AgentId == user.id)
                                            join p in _context.Catalogues on hp.CatalogueId equals p.Id
                                            join i in _context.DigitalProductImages on hp.Id equals i.ProductId
                                            select new { p, hp, i }).GroupBy(z => z.p.Name)
                                            .Select(d => new SellingProduct
                                            {
                                                Name = d.Key,
                                                Image = d.Select(e => e.i.ImageUrl).FirstOrDefault()
                                            }).Take(10).ToList(),
                         ReviewCount = (from hp in _context.HubDigitalProducts.Where(s => s.AgentId == user.id)
                                        join r in _context.HubReviews on hp.Id equals r.ProductId
                                        where r.IsDigital == true
                                        select r).Select(r => r.ProductId).Count(),
                         MaxRating = (from hp in _context.HubDigitalProducts.Where(s => s.AgentId == user.id)
                                      join r in _context.HubReviews on hp.Id equals r.ProductId
                                      where r.IsDigital == true
                                      select r.Rating).OrderByDescending(r => r).FirstOrDefault(),
                         Experience = _context.HubAgentProfiles.Where(r => r.UserId == user.id).Select(e => e.Experience).FirstOrDefault(),
                         AgentsAddress = _context.ShippingAddresses.Where(r => r.CustomerId == user.id && (request.Location == null || r.State.Contains(request.Location))).Select(n => new AgentsAddress
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
            uquery = uquery.Where(e => e.SellingProducts.Any(r => r.Name!.Contains(request.ProductName)));
        }
        return uquery;
    }

    public IQueryable<DigitalProductDetailsResponse> GetAgentDigitalProducts(int catalogueId, int agentId)
    {
        var query = (from ph in _context.HubDigitalProducts.Where(d => d.AgentId == agentId)
                     join img in _context.DigitalProductImages on ph.Id equals img.ProductId
                     join c in _context.Catalogues on ph.CatalogueId equals c.Id
                     where ph.CatalogueId == catalogueId || catalogueId == 0
                     orderby ph.DateCreated descending
                     select new { ph, img, c }).GroupBy(d => d.img.ProductId).Select(f => new DigitalProductDetailsResponse
                     {
                         ProductId = f.Key,
                         Title = f.Select(e => e.ph.Title).FirstOrDefault(),
                         Name = f.Select(e => e.c.Name).FirstOrDefault(),
                         CatalogueId = f.Select(e => e.c.Id).FirstOrDefault(),
                         Price = f.Select(e => e.ph.Price).FirstOrDefault(),
                         Description = f.Select(e => e.ph.Description).FirstOrDefault(),
                         ImageUrl = f.Select(e => e.img.ImageUrl).FirstOrDefault(),
                         ReviewCount = _context.HubReviews.Where(d => d.IsDigital == true && d.ProductId == f.Key).Count(),
                         MaxRating = _context.HubReviews.Where(d => d.IsDigital == true && d.ProductId == f.Key).Sum(d => d.Rating) / 100
                     });
        return query;
    }

    public IQueryable<DigitalProductDetailsResponse> GetPublicAgentDigitalProducts(int catalogueId, int agentId)
    {
        var query = (from user in _context.userstb
                     where user.IsAgent == true && user.IsPublicAgent == true && user.id == agentId
                     join ph in _context.HubDigitalProducts on user.id equals ph.AgentId
                     join img in _context.DigitalProductImages on ph.Id equals img.ProductId
                     join c in _context.Catalogues on ph.CatalogueId equals c.Id
                     where ph.CatalogueId == catalogueId || catalogueId == 0
                     orderby ph.DateCreated descending
                     select new { ph, img, c }).GroupBy(d => d.img.ProductId).Select(f => new DigitalProductDetailsResponse
                     {
                         ProductId = f.Key,
                         Title = f.Select(e => e.ph.Title).FirstOrDefault(),
                         Name = f.Select(e => e.c.Name).FirstOrDefault(),
                         CatalogueId = f.Select(e => e.c.Id).FirstOrDefault(),
                         Price = f.Select(e => e.ph.Price).FirstOrDefault(),
                         Description = f.Select(e => e.ph.Description).FirstOrDefault(),
                         ImageUrl = f.Select(e => e.img.ImageUrl).FirstOrDefault(),
                         ReviewCount = _context.HubReviews.Where(d => d.IsDigital == true && d.ProductId == f.Key).Count(),
                         MaxRating = _context.HubReviews.Where(d => d.IsDigital == true && d.ProductId == f.Key).Sum(d => d.Rating) / 100
                     });
        return query;
    }

    public async Task<DigitalProductDetailResponse> GetAgentDigitalProduct(long productId)
    {
        var query = await (from ph in _context.HubDigitalProducts.Where(d => d.Id == productId)
                           join user in _context.userstb on ph.AgentId equals user.id
                           join img in _context.DigitalProductImages on ph.Id equals img.ProductId
                           join p in _context.Catalogues on ph.CatalogueId equals p.Id
                           select new { ph, img, p, user }).GroupBy(d => d.img.ProductId).Select(f => new DigitalProductDetailResponse
                           {
                               ProductId = f.Key,
                               Title = f.Select(e => e.ph.Title).FirstOrDefault(),
                               Name = f.Select(e => e.p.Name).FirstOrDefault(),
                               AgentName = f.Select(e => e.user.BusinessName).FirstOrDefault(),
                               Price = f.Select(e => e.ph.Price).FirstOrDefault(),
                               Description = f.Select(e => e.ph.Description).FirstOrDefault(),
                               ImageUrl = f.Select(e => e.img.ImageUrl).ToList(),
                               ReviewCount = _context.HubReviews.Where(d => d.IsDigital == true && d.ProductId == f.Key).Count(),
                               MaxRating = _context.HubReviews.Where(d => d.IsDigital == true && d.ProductId == f.Key).Sum(d => d.Rating) / 100
                           }).FirstOrDefaultAsync();
        return query ?? new DigitalProductDetailResponse { };
    }
}
