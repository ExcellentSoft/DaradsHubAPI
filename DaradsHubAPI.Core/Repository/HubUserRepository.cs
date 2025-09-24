using DaradsHubAPI.Core.IRepository;
using DaradsHubAPI.Core.Model.Request;
using DaradsHubAPI.Core.Model.Response;
using DaradsHubAPI.Core.Services.Interface;
using DaradsHubAPI.Domain.Entities;
using DaradsHubAPI.Infrastructure;
using DaradsHubAPI.Shared.Customs;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DaradsHubAPI.Core.Repository;
public class HubUserRepository(AppDbContext _context) : GenericRepository<userstb>(_context), IHubUserRepository
{
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