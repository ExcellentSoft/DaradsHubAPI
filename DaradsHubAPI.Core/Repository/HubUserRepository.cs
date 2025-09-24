using DaradsHubAPI.Core.IRepository;
using DaradsHubAPI.Core.Model.Response;
using DaradsHubAPI.Domain.Entities;
using DaradsHubAPI.Infrastructure;
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
}