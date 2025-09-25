using DaradsHubAPI.Core.IRepository;
using DaradsHubAPI.Core.Model.Request;
using DaradsHubAPI.Core.Model.Response;
using DaradsHubAPI.Domain.Entities;
using DaradsHubAPI.Infrastructure;
using DaradsHubAPI.Shared.Customs;
using Microsoft.EntityFrameworkCore;

namespace DaradsHubAPI.Core.Repository;
public class HubUserRepository(AppDbContext _context) : GenericRepository<userstb>(_context), IHubUserRepository
{
    public IQueryable<AgentsListResponse> GetAgents(AgentsListRequest request)
    {
        var today = GetLocalDateTime.CurrentDateTime();
        var uquery = from user in _context.userstb.Where(d => d.IsAgent == true)
                     let cansellPhysical = _context.HubAgentProductSettings.Where(s => s.AgentId == user.id).Select(e => e.CanSellPhysicalProduct).FirstOrDefault()
                     let cansellDigital = _context.HubAgentProductSettings.Where(s => s.AgentId == user.id).Select(e => e.CanSellDigitalProduct).FirstOrDefault()
                     select new AgentsListResponse
                     {
                         FullName = user.fullname,
                         LastActive = CustomizeCodes.GetPeriodDifference(user.ModifiedDate, today),
                         IsPublic = user.IsPublicAgent,
                         CanSellPhysicalProducts = cansellPhysical,
                         CanSellDigitalProducts = cansellDigital,
                         Status = user.status,
                         AgentId = user.id,
                         Photo = user.Photo,
                         OrderCount = (from item in _context.HubOrderItems
                                       join order in _context.HubOrders on item.OrderCode equals order.Code
                                       where item.AgentId == user.id
                                       select order.Code).Distinct().Count(),
                         RevenueAmount = _context.wallettb.Where(s => s.UserId == user.email).Select(d => d.Balance).FirstOrDefault(),
                         MaxRating = (from hp in _context.HubDigitalProducts.Where(s => s.AgentId == user.id)
                                      join r in _context.HubReviews on hp.Id equals r.ProductId
                                      where r.IsDigital == true
                                      select r.Rating).OrderByDescending(r => r).FirstOrDefault() == 0 ?
                                      (from hp in _context.HubAgentProducts.Where(s => s.AgentId == user.id)
                                       join r in _context.HubReviews on hp.Id equals r.ProductId
                                       where r.IsDigital == false
                                       select r.Rating).OrderByDescending(r => r).FirstOrDefault() : 0
                     };

        if (request.CanSellPhysicalProducts is not null)
        {
            uquery = uquery.Where(e => e.CanSellPhysicalProducts);
        }
        if (request.CanSellDigitalProducts is not null)
        {
            uquery = uquery.Where(e => e.CanSellDigitalProducts);
        }
        if (request.Status is not null)
        {
            uquery = uquery.Where(e => e.Status == request.Status);
        }
        if (request.IsPublic is not null)
        {
            uquery = uquery.Where(e => e.IsPublic == request.IsPublic);
        }
        if (!string.IsNullOrWhiteSpace(request.SearchText))
        {
            uquery = uquery.Where(e => e.FullName != null && e.FullName.ToLower().Contains(request.SearchText.ToLower()));
        }
        return uquery;
    }

    public async Task<(bool status, string message, ShortAgentProfileResponse? res)> GetAgentProductProfile(int agentId)
    {
        var customerUser = await _context.userstb.FirstOrDefaultAsync(us => us.id == agentId);
        if (customerUser is null)
            return new(false, "Agent record not found.", null);

        var response = new ShortAgentProfileResponse
        {
            FullName = customerUser.fullname,
            PhoneNumber = customerUser.phone,
            Photo = customerUser.Photo,
            Code = $"AGT-{customerUser.id}",
            Address = _context.ShippingAddresses.Where(d => d.CustomerId == customerUser.id).Select(d => new AgentAddress
            {
                Address = d.Address,
                Country = d.Country,
                State = d.State,
                City = d.City,
            }).FirstOrDefault()
        };
        var physicalProduct = await _context.HubAgentProducts.Where(e => e.AgentId == agentId).CountAsync();
        var digitalProduct = await _context.HubDigitalProducts.Where(e => e.AgentId == agentId).CountAsync();

        response.TotalProductCount = physicalProduct + digitalProduct;

        return new(true, "Successful.", response);
    }

    public async Task<(bool status, string message)> UpdateAgentVisibility(AgentVisibilityRequest request)
    {
        var agent = await _context.userstb.Where(d => d.id == request.AgentId).FirstOrDefaultAsync();

        if (agent is null)
        {
            return new(false, $"Agent record not found");
        }

        agent.IsPublicAgent = request.IsPublic;

        bool canSellDigitalProduct = false;
        bool canSellPhysicalProduct = false;

        if (request.CataloguesIds is not null)
        {
            await _context.CatalogueMappings.Where(e => e.AgentId == agent.id).ExecuteDeleteAsync();

            foreach (var id in request.CataloguesIds)
            {
                var map = new CatalogueMapping
                {
                    AgentId = agent.id,
                    CatalogueId = id
                };
                _context.CatalogueMappings.Add(map);
            }
            canSellDigitalProduct = true;
            await _context.SaveChangesAsync();
        }

        if (request.CategoriesIds is not null)
        {
            await _context.CategoryMappings.Where(e => e.AgentId == agent.id).ExecuteDeleteAsync();

            foreach (var id in request.CategoriesIds)
            {
                var map = new CategoryMapping
                {
                    AgentId = agent.id,
                    CategoryId = id
                };
                _context.CategoryMappings.Add(map);
            }
            canSellPhysicalProduct = true;
            await _context.SaveChangesAsync();
        }

        var settings = await _context.HubAgentProductSettings.FirstOrDefaultAsync(s => s.AgentId == agent.id);
        if (settings is null)
        {
            settings = new HubAgentProductSetting
            {
                CanSellDigitalProduct = canSellDigitalProduct,
                CanSellPhysicalProduct = canSellPhysicalProduct,
                DateCreated = GetLocalDateTime.CurrentDateTime(),
                MaximumProduct = request.ProductLimit,
                AgentId = agent.id
            };

            _context.HubAgentProductSettings.Add(settings);
        }
        else
        {
            settings.CanSellDigitalProduct = canSellDigitalProduct;
            settings.CanSellPhysicalProduct = canSellPhysicalProduct;
            settings.MaximumProduct = request.ProductLimit;
        }

        await _context.SaveChangesAsync();
        return new(true, $"Agent visibility updated successfully.");
    }
}