using DaradsHubAPI.Core.IRepository;
using DaradsHubAPI.Core.Model.Request;
using DaradsHubAPI.Core.Model.Response;
using DaradsHubAPI.Domain.Entities;
using DaradsHubAPI.Infrastructure;
using DaradsHubAPI.Shared.Customs;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using static DaradsHubAPI.Domain.Enums.Enum;

namespace DaradsHubAPI.Core.Repository;
public class HubUserRepository(AppDbContext _context) : GenericRepository<userstb>(_context), IHubUserRepository
{
    public IQueryable<AgentsListResponse> GetAgents(AgentsListRequest request)
    {
        var today = GetLocalDateTime.CurrentDateTime();
        var uquery = from user in _context.userstb.Where(d => d.IsAgent == true)
                     let cansellPhysical = _context.HubAgentProductSettings.Where(s => s.AgentId == user.id).Select(e => e.CanSellPhysicalProduct).FirstOrDefault()
                     let cansellDigital = _context.HubAgentProductSettings.Where(s => s.AgentId == user.id).Select(e => e.CanSellDigitalProduct).FirstOrDefault()
                     orderby user.regdate descending
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
    public IQueryable<CustomersListResponse> GetCustomers(CustomersListRequest request)
    {
        var uquery = from user in _context.AspNetUsers
                     where user.Is_customer == 1
                     join u in _context.userstb on user.Id equals u.userid

                     where u.regdate != null && (request.StartDate == null || u.regdate.Value.Date >= request.StartDate.Value.Date) &&
                                     (request.EndDate == null || u.regdate.Value.Date <= request.EndDate.Value.Date)
                     orderby u.regdate descending

                     select new CustomersListResponse
                     {
                         FullName = u.fullname,
                         JoinDate = u.regdate,
                         Status = user.Status,
                         Email = user.Email,
                         CustomerId = u.id,
                         Photo = u.Photo,
                         Balance = _context.wallettb.Where(s => s.UserId == user.Email).Select(d => d.Balance).FirstOrDefault(),
                     };

        if (request.Status is not null)
        {
            uquery = uquery.Where(e => e.Status == request.Status);
        }
        if (!string.IsNullOrWhiteSpace(request.SearchText))
        {
            uquery = uquery.Where(e => e.FullName != null && e.FullName.ToLower().Contains(request.SearchText.ToLower()));
        }
        return uquery;
    }

    public IQueryable<ReportedAgentsResponse> GetReportedAgents()
    {
        var uquery = from r in _context.ReportAgents
                     orderby r.ReportedDate descending

                     select new ReportedAgentsResponse
                     {
                         Reason = r.Reason,
                         ReportedDate = r.ReportedDate,
                         AgentData = _context.userstb.Where(x => x.id == r.AgentId).Select(e => new UserDatum
                         {
                             FullName = e.fullname,
                             Photo = e.Photo,
                             PhoneNumber = e.phone
                         }).FirstOrDefault(),
                         CustomerData = _context.userstb.Where(x => x.id == r.CustomerId).Select(e => new UserDatum
                         {
                             FullName = e.fullname,
                             Photo = e.Photo,
                             PhoneNumber = e.phone
                         }).FirstOrDefault()
                     };
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
            JoinedDate = customerUser.regdate,
            Email = customerUser.email,
            IsPublic = customerUser.IsPublicAgent,
            Experience = customerUser.AgentExperience,
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
        var query = from hp in _context.HubAgentProducts.Where(s => s.AgentId == agentId)
                    join p in _context.categories on hp.CategoryId equals p.id
                    select new { hp, p };

        var dquery = from hp in _context.HubDigitalProducts.Where(s => s.AgentId == agentId)
                     join p in _context.Catalogues on hp.CatalogueId equals p.Id
                     select new { p };

        var rQuery = from hp in _context.HubAgentProducts.Where(s => s.AgentId == agentId)
                     join r in _context.HubReviews on hp.Id equals r.ProductId
                     select new { r.Rating, r.ProductId };

        response.SellingProducts = await query.Select(d => d.p.name).Distinct().Take(10).ToListAsync();
        response.AnotherSellingProducts = await dquery.Select(d => d.p.Name).Distinct().Take(10).ToListAsync();
        response.ReviewCount = rQuery.Select(r => r.ProductId).Count();
        response.MaxRating = rQuery.Select(w => w.Rating).OrderByDescending(r => r).FirstOrDefault();

        var physicalProduct = await _context.HubAgentProducts.Where(e => e.AgentId == agentId).CountAsync();
        var digitalProduct = await _context.HubDigitalProducts.Where(e => e.AgentId == agentId).CountAsync();

        response.TotalProductCount = physicalProduct + digitalProduct;

        return new(true, "Successful.", response);
    }

    public async Task<(bool status, string message, ShortCustomerProfileResponse? res)> GetCustomerProfile(int customerId)
    {
        var customerUser = await _context.userstb.FirstOrDefaultAsync(us => us.id == customerId);
        if (customerUser is null)
            return new(false, "Customer record not found.", null);

        var response = new ShortCustomerProfileResponse
        {
            FullName = customerUser.fullname,
            PhoneNumber = customerUser.phone,
            JoinedDate = customerUser.regdate,
            Email = customerUser.email,
            IsOnline = customerUser.IsOnline,
            Photo = customerUser.Photo,
            Code = $"CUS-{customerUser.id}",
            Address = _context.ShippingAddresses.Where(d => d.CustomerId == customerUser.id).Select(d => new AgentAddress
            {
                Address = d.Address,
                Country = d.Country,
                State = d.State,
                City = d.City,
            }).FirstOrDefault(),
            Status = customerUser.status,
            CustomerId = customerUser.id,
            WalletBalance = _context.wallettb.Where(s => s.UserId == customerUser.email).Select(x => x.Balance).FirstOrDefault(),
            TotalOrderMade = _context.HubOrders.Where(c => c.UserEmail == customerUser.email).Count(),
            TotalSpending = _context.GwalletTrans.Where(x => x.DR > 0 && x.userName == customerUser.email).Select(w => w.amt).Sum()


        };

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

    public async Task<(bool status, string message, bool IsPublic)> ToggleVisibility(int agentId, bool IsPublic)
    {
        var agent = await _context.userstb.Where(d => d.id == agentId).FirstOrDefaultAsync();

        if (agent is null)
        {
            return new(false, $"Agent record not found", false);
        }

        agent.IsPublicAgent = IsPublic;

        await _context.SaveChangesAsync();
        return new(true, $"Successful", agent.IsPublicAgent.GetValueOrDefault());
    }

    public async Task<CustomerMetricsResponse> GetCustomerMetrics()
    {
        var currentDate = GetLocalDateTime.CurrentDateTime();
        var last30Days = GetLocalDateTime.CurrentDateTime().AddDays(-30);
        var user = _context.AspNetUsers.Where(c => c.Is_customer == 1).AsNoTracking();

        var metrics = new CustomerMetricsResponse();
        if (user.Any())
        {
            var newCustomerCount = (from u in user
                                    join c in _context.userstb on u.Id equals c.userid
                                    where c.regdate != null && c.regdate.Value.Year == currentDate.Year && c.regdate.Value.Month == currentDate.Month
                                    select c).Count();

            var activeChat = await (from u in user
                                    join c in _context.userstb on u.Id equals c.userid
                                    join m in _context.HubChatMessages on c.id equals m.SenderId
                                    where m.SentAt >= last30Days
                                    select m).GroupBy(g => g.SenderId).CountAsync();

            metrics = new CustomerMetricsResponse
            {
                TotalCustomerCount = await user.CountAsync(),
                TotalInActiveCount = user.Where(e => e.Status == EntityStatusEnum.InActive).Count(),
                NewCustomerModel = new NewCustomerModel
                {
                    TotalNewCustomerCount = newCustomerCount,
                    Month = currentDate.ToString("MMM")
                },
                TotalActiveChatCount = activeChat
            };

            return metrics;
        }
        return metrics;
    }
}