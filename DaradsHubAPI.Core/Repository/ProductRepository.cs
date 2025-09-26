using DaradsHubAPI.Core.IRepository;
using DaradsHubAPI.Core.Model.Request;
using DaradsHubAPI.Core.Model.Response;
using DaradsHubAPI.Domain.Entities;
using DaradsHubAPI.Infrastructure;
using DaradsHubAPI.Shared.Static;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using static DaradsHubAPI.Domain.Enums.Enum;

namespace DaradsHubAPI.Core.Repository;
public class ProductRepository(AppDbContext _context) : GenericRepository<HubAgentProduct>(_context), IProductRepository
{
    #region Admin Queries

    public async Task<List<CustomerRequestResponse>> GetCustomerRequestsForAdmin(CustomerRequestsRequest request)
    {
        var qOrder = from req in _context.HubProductRequests
                     where (request.StartDate == null || req.DateCreated >= request.StartDate.Value.Date) &&
                        (request.EndDate == null || req.DateCreated <= request.EndDate.Value.Date)
                     orderby req.DateCreated descending
                     select new CustomerRequestResponse
                     {
                         IsUrgent = req.IsUrgent,
                         DateCreated = req.DateCreated,
                         Location = req.Location,
                         PreferredDate = req.PreferredDate,
                         ProductService = req.CustomerNeed,
                         Quantity = req.Quantity,
                         Reference = $"#REQ-{req.Id}",
                         RequestId = req.Id,
                         ProductServiceImageUrl = _context.ProductRequestImages.Where(d => d.RequestId == req.Id).Select(u => u.ImageUrl).FirstOrDefault(),
                         Customer = _context.userstb.Where(r => r.id == req.CustomerId).Select(e => new RequestedUser
                         {
                             FullName = e.fullname,
                             Photo = e.Photo
                         }).FirstOrDefault(),
                         Status = req.Status,
                     };

        var res = new List<CustomerRequestResponse>();
        if (qOrder.Any())
        {
            if (!string.IsNullOrEmpty(request.SearchText))
            {
                var searchText = request.SearchText.Trim().ToLower();

                qOrder = qOrder.Where(d => d.ProductService.ToLower().Contains(request.SearchText));
            }

            if (request.Status is not null)
            {
                qOrder = qOrder.Where(d => d.Status == request.Status);
            }

            res = await qOrder.Skip((request!.PageNumber - 1) * request.PageSize).Take(request.PageSize).OrderByDescending(d => d.DateCreated).ToListAsync();
        }
        return res;


    }
    public async Task<CustomerRequestMetricResponse> GetCustomerRequestMetricsForAdmin()
    {
        var query = from req in _context.HubProductRequests
                    select req;

        var metrics = new CustomerRequestMetricResponse();
        if (query.Any())
        {
            metrics = new CustomerRequestMetricResponse
            {
                TotalCount = query.Count(),
                PendingCount = query.Where(r => r.Status == RequestStatus.Pending).Count(),
                ApproveCount = query.Where(r => r.Status == RequestStatus.Approved).Count(),
                RejectCount = query.Where(r => r.Status == RequestStatus.Rejected).Count()
            };
        }

        return await Task.FromResult(metrics);
    }

    #endregion

    #region Physical Product    
    public IQueryable<HubProduct> GetHubProducts(string? searchText)
    {
        searchText = searchText?.Trim().ToLower();
        var hubProducts = _context.HubProducts.Where(s => searchText == null || s.Name.ToLower().Contains(searchText));
        return hubProducts;
    }
    public async Task<HubProductRequest?> GetHubProductRequest(long id)
    {
        var request = await _context.HubProductRequests.Where(s => s.Id == id).FirstOrDefaultAsync();
        return request;
    }
    public async Task AddHubProductImages(ProductImages productImages)
    {
        _context.ProductImages.Add(productImages);
        await Task.CompletedTask;
    }
    public async Task AddHubProductRequestImages(ProductRequestImages productImages)
    {
        _context.ProductRequestImages.Add(productImages);
        await Task.CompletedTask;
    }

    public async Task CreateHubProductRequest(HubProductRequest request)
    {
        _context.HubProductRequests.Add(request);
        await _context.SaveChangesAsync();
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
                         ImageUrls = f.Select(e => e.img.ImageUrl).ToList()
                     });
        return query;
    }

    public IQueryable<LandingProductResponse> GetPublicLandPageProducts()
    {
        var query = (from user in _context.userstb
                     where user.IsAgent == true && user.IsPublicAgent == true
                     join ph in _context.HubAgentProducts on user.id equals ph.AgentId
                     join img in _context.ProductImages on ph.Id equals img.ProductId
                     orderby ph.DateCreated descending
                     select new { ph, img }).GroupBy(d => d.img.ProductId).Select(f => new LandingProductResponse
                     {
                         Id = f.Key,
                         Caption = f.Select(e => e.ph.Caption).FirstOrDefault(),
                         AgentId = f.Select(e => e.ph.AgentId).FirstOrDefault(),
                         Description = f.Select(e => e.ph.Description).FirstOrDefault(),
                         ImageUrls = f.Select(e => e.img.ImageUrl).ToList()
                     });
        return query;
    }

    public async Task<CustomerRequestMetricResponse> GetCustomerRequestMetrics(long agentId)
    {
        var query = from req in _context.HubProductRequests
                    where req.AgentId == agentId
                    select req;

        var metrics = new CustomerRequestMetricResponse();
        if (query.Any())
        {
            metrics = new CustomerRequestMetricResponse
            {
                TotalCount = query.Count(),
                PendingCount = query.Where(r => r.Status == RequestStatus.Pending).Count(),
                ApproveCount = query.Where(r => r.Status == RequestStatus.Approved).Count(),
                RejectCount = query.Where(r => r.Status == RequestStatus.Rejected).Count()
            };
        }

        return await Task.FromResult(metrics);
    }

    public async Task<List<CustomerRequestResponse>> GetCustomerRequests(CustomerRequestsRequest request, int agentId)
    {
        var qOrder = from req in _context.HubProductRequests
                     where req.AgentId == agentId && (request.StartDate == null || req.DateCreated >= request.StartDate.Value.Date) &&
                        (request.EndDate == null || req.DateCreated <= request.EndDate.Value.Date)
                     orderby req.DateCreated descending
                     select new CustomerRequestResponse
                     {
                         IsUrgent = req.IsUrgent,
                         DateCreated = req.DateCreated,
                         Location = req.Location,
                         PreferredDate = req.PreferredDate,
                         ProductService = req.CustomerNeed,
                         Quantity = req.Quantity,
                         Reference = $"#REQ-{req.Id}",
                         RequestId = req.Id,
                         ProductServiceImageUrl = _context.ProductRequestImages.Where(d => d.RequestId == req.Id).Select(u => u.ImageUrl).FirstOrDefault(),
                         Customer = _context.userstb.Where(r => r.id == req.CustomerId).Select(e => new RequestedUser
                         {
                             FullName = e.fullname,
                             Photo = e.Photo
                         }).FirstOrDefault(),
                         Status = req.Status,
                     };

        var res = new List<CustomerRequestResponse>();
        if (qOrder.Any())
        {
            if (!string.IsNullOrEmpty(request.SearchText))
            {
                var searchText = request.SearchText.Trim().ToLower();

                qOrder = qOrder.Where(d => d.ProductService.ToLower().Contains(request.SearchText));
            }

            if (request.Status is not null)
            {
                qOrder = qOrder.Where(d => d.Status == request.Status);
            }

            res = await qOrder.Skip((request!.PageNumber - 1) * request.PageSize).Take(request.PageSize).OrderByDescending(d => d.DateCreated).ToListAsync();
        }
        return res;
    }

    public async Task<SingleCustomerRequestResponse?> GetCustomerRequest(long requestId)
    {
        var request = await (from req in _context.HubProductRequests
                             where req.Id == requestId
                             select new SingleCustomerRequestResponse
                             {
                                 IsUrgent = req.IsUrgent,
                                 DateCreated = req.DateCreated,
                                 Location = req.Location,
                                 PreferredDate = req.PreferredDate,
                                 ProductService = req.CustomerNeed,
                                 Quantity = req.Quantity,
                                 Budget = req.Budget,
                                 Reference = $"#REQ-{req.Id}",
                                 RequestId = req.Id,
                                 ProductServiceImageUrls = _context.ProductRequestImages.Where(d => d.RequestId == req.Id).Select(u => u.ImageUrl).ToList(),
                                 ProductType = req.ProductRequestTypeEnum.GetDescription(),
                                 Category = req.ProductRequestTypeEnum == ProductRequestTypeEnum.Digital ? _context.Catalogues.Where(e => e.Id == req.CategoryId).Select(w => w.Name).FirstOrDefault() : _context.categories.Where(e => e.id == req.CategoryId).Select(w => w.name).FirstOrDefault(),
                                 Customer = _context.userstb.Where(r => r.id == req.CustomerId).Select(e => new RequestedUser
                                 {
                                     FullName = e.fullname,
                                     Photo = e.Photo
                                 }).FirstOrDefault(),
                                 Status = req.Status,
                             }).FirstOrDefaultAsync();
        return request;
    }

    public IQueryable<AgentProductsResponse> GetDigiatlProducts(AgentProductsRequest request, int agentId)
    {
        var query = (from ph in _context.HubDigitalProducts
                     where ph.AgentId == agentId
                     join p in _context.Catalogues on ph.CatalogueId equals p.Id
                     join img in _context.DigitalProductImages on ph.Id equals img.ProductId
                     orderby ph.DateCreated descending
                     select new { ph, img, p }).GroupBy(d => d.ph.Id).Select(f => new AgentProductsResponse
                     {
                         ProductId = f.Key,
                         Name = f.Select(e => e.p.Name).FirstOrDefault() ?? "",
                         Caption = f.Select(e => e.ph.Title).FirstOrDefault() ?? "",
                         Price = f.Select(e => e.ph.Price).FirstOrDefault(),
                         ProductType = "Digital",
                         Orders = (from item in _context.HubOrderItems
                                   where item.ProductId == f.Key
                                   join order in _context.HubOrders on item.OrderCode equals order.Code
                                   where order.ProductType == "Digital"
                                   select new { order }).Count(),
                         Stock = 1,
                         UpdatedDate = f.Select(e => e.ph.DateUpdated).FirstOrDefault(),
                         Description = f.Select(e => e.ph.Description).FirstOrDefault(),
                         ImageUrls = f.Select(e => e.img.ImageUrl).ToList()
                     });

        if (!string.IsNullOrEmpty(request.ProductType))
        {
            request.ProductType = request.ProductType.Trim().ToLower();
            query = query.Where(x => x.ProductType.ToLower() == request.ProductType);
        }
        if (!string.IsNullOrEmpty(request.SearchText))
        {
            request.SearchText = request.SearchText.Trim().ToLower();
            query = query.Where(x => x.Name.ToLower().Contains(request.SearchText) || x.Caption.ToLower().Contains(request.SearchText));
        }

        return query;
    }

    public IQueryable<AgentProductsResponse> GetPhysicalProducts(AgentProductsRequest request, int agentId)
    {
        var query = (from ph in _context.HubAgentProducts
                     where ph.AgentId == agentId
                     join p in _context.HubProducts on ph.ProductId equals p.Id
                     join img in _context.ProductImages on ph.Id equals img.ProductId
                     orderby ph.DateCreated descending
                     select new { ph, img, p }).GroupBy(d => d.ph.Id).Select(f => new AgentProductsResponse
                     {
                         ProductId = f.Key,
                         Name = f.Select(e => e.p.Name).FirstOrDefault() ?? "",
                         Caption = f.Select(e => e.ph.Caption).FirstOrDefault() ?? "",
                         Price = f.Select(e => e.ph.Price).FirstOrDefault(),
                         Stock = f.Select(e => e.ph.Stock).FirstOrDefault(),
                         ProductType = "Physical",
                         Orders = (from item in _context.HubOrderItems
                                   where item.ProductId == f.Key
                                   join order in _context.HubOrders on item.OrderCode equals order.Code
                                   where order.ProductType == "Physical"
                                   select new { order }).Count(),
                         UpdatedDate = f.Select(e => e.ph.DateUpdated).FirstOrDefault(),
                         Description = f.Select(e => e.ph.Description).FirstOrDefault(),
                         ImageUrls = f.Select(e => e.img.ImageUrl).ToList()
                     });

        if (!string.IsNullOrEmpty(request.ProductType))
        {
            request.ProductType = request.ProductType.Trim().ToLower();
            query = query.Where(x => x.ProductType.ToLower() == request.ProductType);
        }
        if (!string.IsNullOrEmpty(request.SearchText))
        {
            request.SearchText = request.SearchText.Trim().ToLower();
            query = query.Where(x => x.Name.ToLower().Contains(request.SearchText) || x.Caption.ToLower().Contains(request.SearchText));
        }

        return query;
    }

    public IQueryable<category> GetAgentCategories(string? searchText, int agentId)
    {
        searchText = searchText?.Trim().ToLower();

        var query = from mapping in _context.CategoryMappings
                    where mapping.AgentId == agentId
                    join cate in _context.categories on mapping.CategoryId equals cate.id
                    select cate;

        if (!string.IsNullOrEmpty(searchText))
        {
            query = query.Where(d => d.name.ToLower().Contains(searchText));
        }
        return query;
    }

    public IQueryable<AgentProductsResponse> GetProducts(AgentProductsRequest request, int agentId)
    {
        var physicalQuery = (from ph in _context.HubAgentProducts
                             where ph.AgentId == agentId
                             join p in _context.HubProducts on ph.ProductId equals p.Id
                             join img in _context.ProductImages on ph.Id equals img.ProductId
                             orderby ph.DateCreated descending
                             select new { ph, img, p }).GroupBy(d => d.ph.Id).Select(f => new AgentProductsResponse
                             {
                                 ProductId = f.Key,
                                 Name = f.Select(e => e.p.Name).FirstOrDefault() ?? "",
                                 Caption = f.Select(e => e.ph.Caption).FirstOrDefault() ?? "",
                                 Price = f.Select(e => e.ph.Price).FirstOrDefault(),
                                 Stock = f.Select(e => e.ph.Stock).FirstOrDefault(),
                                 ProductType = "Physical",
                                 Orders = (from item in _context.HubOrderItems
                                           where item.ProductId == f.Key
                                           join order in _context.HubOrders on item.OrderCode equals order.Code
                                           where order.ProductType == "Physical"
                                           select new { order }).Count(),
                                 UpdatedDate = f.Select(e => e.ph.DateUpdated).FirstOrDefault(),
                                 Description = f.Select(e => e.ph.Description).FirstOrDefault(),
                                 // TempImageUrl = string.Join(',', f.Select(e => e.img.ImageUrl))
                             });

        var digitalQuery = (from ph in _context.HubDigitalProducts
                            where ph.AgentId == agentId
                            join p in _context.Catalogues on ph.CatalogueId equals p.Id
                            join img in _context.DigitalProductImages on ph.Id equals img.ProductId
                            orderby ph.DateCreated descending
                            select new { ph, img, p }).GroupBy(d => d.ph.Id).Select(f => new AgentProductsResponse
                            {
                                ProductId = f.Key,
                                Name = f.Select(e => e.p.Name).FirstOrDefault() ?? "",
                                Caption = f.Select(e => e.ph.Title).FirstOrDefault() ?? "",
                                Price = f.Select(e => e.ph.Price).FirstOrDefault(),
                                ProductType = "Digital",
                                Orders = (from item in _context.HubOrderItems
                                          where item.ProductId == f.Key
                                          join order in _context.HubOrders on item.OrderCode equals order.Code
                                          where order.ProductType == "Digital"
                                          select new { order }).Count(),
                                Stock = 1,
                                UpdatedDate = f.Select(e => e.ph.DateUpdated).FirstOrDefault(),
                                Description = f.Select(e => e.ph.Description).FirstOrDefault(),
                                //TempImageUrl = string.Join(',', f.Select(e => e.img.ImageUrl))
                            });


        var query = physicalQuery.Union(digitalQuery);

        if (!string.IsNullOrEmpty(request.ProductType))
        {
            request.ProductType = request.ProductType.Trim().ToLower();
            query = query.Where(x => x.ProductType.ToLower() == request.ProductType);
        }
        if (!string.IsNullOrEmpty(request.SearchText))
        {
            request.SearchText = request.SearchText.Trim().ToLower();
            query = query.Where(x => x.Name.ToLower().Contains(request.SearchText) || x.Caption.ToLower().Contains(request.SearchText));
        }

        return query;
    }

    public async Task<ProductOrderMetricResponse> GetDigitalProductOrderMetrics(long productId)
    {
        var query = from item in _context.HubOrderItems
                    where item.ProductId == productId
                    join order in _context.HubOrders on item.OrderCode equals order.Code
                    where order.ProductType == "Digital"
                    select new { order };

        var metrics = new ProductOrderMetricResponse();
        if (query.Any())
        {
            var queryD = query.GroupBy(d => d.order.Code);
            metrics = new ProductOrderMetricResponse
            {
                TotalOrderCount = queryD.Select(d => d.Key).Count(),
                PendingOrderCount = queryD.Where(r => r.Select(e => e.order.Status).FirstOrDefault() == OrderStatus.Order).Count(),
                CompletedOrderCount = queryD.Where(r => r.Select(e => e.order.Status).FirstOrDefault() == OrderStatus.Completed).Count(),
                CanceledOrderCount = queryD.Where(r => r.Select(e => e.order.Status).FirstOrDefault() == OrderStatus.Cancelled).Count(),
                RefundedOrderCount = queryD.Where(r => r.Select(e => e.order.Status).FirstOrDefault() == OrderStatus.Refunded).Count(),
                ProcessingOrderCount = queryD.Where(r => r.Select(e => e.order.Status).FirstOrDefault() == OrderStatus.Processing).Count(),
            };
        }

        return await Task.FromResult(metrics);
    }
    public async Task<ProductOrderMetricResponse> GetPhysicalProductOrderMetrics(long productId)
    {
        var query = from item in _context.HubOrderItems
                    where item.ProductId == productId
                    join order in _context.HubOrders on item.OrderCode equals order.Code
                    where order.ProductType == "Physical"
                    select new { order };

        var metrics = new ProductOrderMetricResponse();
        if (query.Any())
        {
            var queryD = query.GroupBy(d => d.order.Code);
            metrics = new ProductOrderMetricResponse
            {
                TotalOrderCount = queryD.Select(d => d.Key).Count(),
                PendingOrderCount = queryD.Where(r => r.Select(e => e.order.Status).FirstOrDefault() == OrderStatus.Order).Count(),
                CompletedOrderCount = queryD.Where(r => r.Select(e => e.order.Status).FirstOrDefault() == OrderStatus.Completed).Count(),
                CanceledOrderCount = queryD.Where(r => r.Select(e => e.order.Status).FirstOrDefault() == OrderStatus.Cancelled).Count(),
                RefundedOrderCount = queryD.Where(r => r.Select(e => e.order.Status).FirstOrDefault() == OrderStatus.Refunded).Count(),
                ProcessingOrderCount = queryD.Where(r => r.Select(e => e.order.Status).FirstOrDefault() == OrderStatus.Processing).Count(),
            };
        }

        return await Task.FromResult(metrics);
    }

    public async Task<List<AgentOrderListResponse>> GetDigitalProductOrders(ProductOrderListRequest request)
    {
        var qOrder = from item in _context.HubOrderItems
                     where item.ProductId == request.ProductId
                     join order in _context.HubOrders on item.OrderCode equals order.Code
                     where order.ProductType == "Digital"
                     where (request.StartDate == null || order.OrderDate.Date >= request.StartDate.Value.Date) &&
                        (request.EndDate == null || order.OrderDate.Date <= request.EndDate.Value.Date)
                     select new { order };
        var res = new List<AgentOrderListResponse>();
        if (qOrder.Any())
        {
            var qq = qOrder.GroupBy(d => d.order.Code).Select(q => new AgentOrderListResponse
            {
                OrderId = q.Select(r => r.order.Id).FirstOrDefault(),
                Code = q.Select(r => r.order.Code).FirstOrDefault(),
                OrderDate = q.Select(r => r.order.OrderDate).FirstOrDefault(),
                OrderStatus = q.Select(r => r.order.Status).FirstOrDefault(),
                ProductType = q.Select(r => r.order.ProductType).FirstOrDefault(),
                TotalPrice = q.Select(r => r.order.TotalCost).FirstOrDefault(),
                CustomerDetails = _context.userstb.Where(e => e.email == q.Select(r => r.order.UserEmail).FirstOrDefault()).Select(d => new CustomerData
                {
                    Email = d.email,
                    Name = d.fullname,
                    PhoneNumber = d.phone
                }).FirstOrDefault()

            });

            if (!string.IsNullOrEmpty(request.SearchText))
            {
                var searchText = request.SearchText.Trim().ToLower();

                qOrder = qOrder.Where(d => d.order.Code.Contains(request.SearchText));
            }

            if (request.Status is not null)
            {
                qOrder = qOrder.Where(d => d.order.Status == request.Status);
            }

            res = await qq.Skip((request!.PageNumber - 1) * request.PageSize).Take(request.PageSize).OrderByDescending(d => d.OrderDate).ToListAsync();
        }
        return res;
    }

    public async Task<List<AgentOrderListResponse>> GetPhysicalProductOrders(ProductOrderListRequest request)
    {
        var qOrder = from item in _context.HubOrderItems
                     where item.ProductId == request.ProductId
                     join order in _context.HubOrders on item.OrderCode equals order.Code
                     where order.ProductType == "Physical"
                     where (request.StartDate == null || order.OrderDate.Date >= request.StartDate.Value.Date) &&
                        (request.EndDate == null || order.OrderDate.Date <= request.EndDate.Value.Date)
                     select new { order };
        var res = new List<AgentOrderListResponse>();
        if (qOrder.Any())
        {
            var qq = qOrder.GroupBy(d => d.order.Code).Select(q => new AgentOrderListResponse
            {
                OrderId = q.Select(r => r.order.Id).FirstOrDefault(),
                Code = q.Select(r => r.order.Code).FirstOrDefault(),
                OrderDate = q.Select(r => r.order.OrderDate).FirstOrDefault(),
                OrderStatus = q.Select(r => r.order.Status).FirstOrDefault(),
                ProductType = q.Select(r => r.order.ProductType).FirstOrDefault(),
                TotalPrice = q.Select(r => r.order.TotalCost).FirstOrDefault(),
                CustomerDetails = _context.userstb.Where(e => e.email == q.Select(r => r.order.UserEmail).FirstOrDefault()).Select(d => new CustomerData
                {
                    Email = d.email,
                    Name = d.fullname,
                    PhoneNumber = d.phone
                }).FirstOrDefault()

            });

            if (!string.IsNullOrEmpty(request.SearchText))
            {
                var searchText = request.SearchText.Trim().ToLower();

                qOrder = qOrder.Where(d => d.order.Code.Contains(request.SearchText));
            }

            if (request.Status is not null)
            {
                qOrder = qOrder.Where(d => d.order.Status == request.Status);
            }

            res = await qq.Skip((request!.PageNumber - 1) * request.PageSize).Take(request.PageSize).OrderByDescending(d => d.OrderDate).ToListAsync();
        }
        return res;
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
                        join p in _context.categories on hp.CategoryId equals p.id
                        select new { hp, p };

            var dquery = from hp in _context.HubDigitalProducts.Where(s => s.AgentId == agentId)
                         join p in _context.Catalogues on hp.CatalogueId equals p.Id
                         select new { p };

            var rQuery = from hp in _context.HubAgentProducts.Where(s => s.AgentId == agentId)
                         join r in _context.HubReviews on hp.Id equals r.ProductId
                         where r.IsDigital == false
                         select new { r };

            response.SellingProducts = await query.Select(d => d.p.name).Distinct().Take(10).ToListAsync();
            response.AnotherSellingProducts = await dquery.Select(d => d.p.Name).Distinct().Take(10).ToListAsync();
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

    public async Task<AgentReviewResponse> GetReviewByAgentId(int agentId)
    {
        var query = from user in _context.userstb
                    where user.IsAgent == true && user.id == agentId
                    join r in _context.HubAgentReviews on user.id equals r.AgentId
                    join u in _context.userstb on r.ReviewById equals u.id
                    select new { r, u };
        var response = new AgentReviewResponse();
        if (query.Any())
        {
            response = new AgentReviewResponse
            {
                Reviews = await query.Select(r => new AgentReview
                {
                    Content = r.r.Content,
                    Rating = r.r.Rating,
                    ReviewBy = r.u.fullname,
                    ReviewerPhoto = r.u.Photo,
                    ReviewDate = r.r.ReviewDate
                }).ToListAsync(),
                TotalReviewCount = await query.CountAsync(),
                RatingAverage = query.Select(r => r.r.Rating).Average()
            };

        }

        return response;
    }

    public IQueryable<AgentReview> GetAgentReviews(AgentReviewRequest request, int agentId)
    {
        var query = from user in _context.userstb
                    where user.IsAgent == true && user.id == agentId
                    join r in _context.HubAgentReviews on user.id equals r.AgentId
                    join u in _context.userstb on r.ReviewById equals u.id
                    orderby r.ReviewDate descending
                    where (request.StartDate == null || r.ReviewDate >= request.StartDate.Value.Date) &&
                       (request.EndDate == null || r.ReviewDate <= request.EndDate.Value.Date)
                    select new { r, u };

        if (!string.IsNullOrWhiteSpace(request.SearchText))
        {
            request.SearchText = request.SearchText.Trim().ToLower();
            query = query.Where(d => d.u.fullname.ToLower().Contains(request.SearchText));
        }
        var response = query.Select(r => new AgentReview
        {
            Content = r.r.Content,
            Rating = r.r.Rating,
            ReviewBy = r.u.fullname,
            ReviewerPhoto = r.u.Photo,
            ReviewDate = r.r.ReviewDate
        });
        return response;
    }

    public async Task<AgentReviewResponse> GetReviewByPubicAgentId(int agentId)
    {
        var query = from user in _context.userstb
                    where user.IsAgent == true && user.IsPublicAgent == true && user.id == agentId
                    join r in _context.HubAgentReviews on user.id equals r.AgentId
                    join u in _context.userstb on r.ReviewById equals u.id
                    select new { r, u };
        var response = new AgentReviewResponse();
        if (query.Any())
        {
            response = new AgentReviewResponse
            {
                Reviews = await query.Select(r => new AgentReview
                {
                    Content = r.r.Content,
                    Rating = r.r.Rating,
                    ReviewBy = r.u.fullname,
                    ReviewerPhoto = r.u.Photo,
                    ReviewDate = r.r.ReviewDate
                }).ToListAsync(),
                TotalReviewCount = await query.CountAsync(),
                RatingAverage = query.Select(r => r.r.Rating).Average()
            };

        }

        return response;
    }

    public async Task<IEnumerable<AgentReview>> GetLatestReview(int agentId)
    {
        var query = from user in _context.userstb
                    where user.id == agentId
                    join r in _context.HubAgentReviews on user.id equals r.AgentId
                    join u in _context.userstb on r.ReviewById equals u.id
                    orderby r.ReviewDate descending
                    select new { r, u };
        var response = new List<AgentReview>();
        if (query.Any())
        {

            response = await query.Select(r => new AgentReview
            {
                Content = r.r.Content,
                Rating = r.r.Rating,
                ReviewBy = r.u.fullname,
                ReviewerPhoto = r.u.Photo,
                ReviewDate = r.r.ReviewDate
            }).Take(4).ToListAsync();

        }

        return response;
    }

    public async Task<IEnumerable<string>> GetPhysicalProductImages(long productId)
    {
        var images = await _context.ProductImages.Where(s => s.ProductId == productId).Select(d => d.ImageUrl).ToListAsync();
        return images;
    }

    public async Task<ProductReviewResponse> GetReviewByProductId(int productId)
    {
        var query = from r in _context.HubReviews
                    where r.ProductId == productId
                    join u in _context.userstb on r.ReviewById equals u.id
                    select new { r, u };

        var response = new ProductReviewResponse
        {
            Reviews = await query.Select(r => new ProductReview
            {
                Content = r.r.Content,
                Rating = r.r.Rating,
                ReviewBy = r.u.fullname,
                ReviewerPhoto = r.u.Photo,
                ReviewDate = r.r.ReviewDate,
                IsDigital = r.r.IsDigital
            }).ToListAsync(),
            TotalReviewCount = await query.CountAsync(),
            RatingAverage = query.Select(r => r.r.Rating).Average()
        };

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
                                            join i in _context.ProductImages on hp.Id equals i.ProductId
                                            join p in _context.HubProducts on hp.ProductId equals p.Id
                                            select new { p, hp, i }).GroupBy(z => z.p.Name)
                                            .Select(d => new SellingProduct
                                            {
                                                Name = d.Key,
                                                Image = d.Select(e => e.i.ImageUrl).FirstOrDefault()
                                            }).Take(10).ToList(),
                         ReviewCount = (from hp in _context.HubAgentProducts.Where(s => s.AgentId == user.id)
                                        join r in _context.HubReviews on hp.Id equals r.ProductId
                                        where r.IsDigital == false
                                        select r).Select(r => r.ProductId).Count(),
                         MaxRating = (from hp in _context.HubAgentProducts.Where(s => s.AgentId == user.id)
                                      join r in _context.HubReviews on hp.Id equals r.ProductId
                                      where r.IsDigital == false
                                      select r.Rating).OrderByDescending(r => r).FirstOrDefault(),
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
            uquery = uquery.Where(e => e.SellingProducts.Any(r => r.Name!.Contains(request.ProductName)));
        }
        if (!string.IsNullOrWhiteSpace(request.Location))
        {
            uquery = uquery.Where(e => e.AgentsAddress != null && e.AgentsAddress.Address != null && e.AgentsAddress.Address.Contains(request.Location) || (e.AgentsAddress!.City != null && e.AgentsAddress.City.Contains(request.Location)) || (e.AgentsAddress!.State != null && e.AgentsAddress.State.Contains(request.Location)));
        }

        return uquery;
    }

    public IQueryable<AgentsProfileResponse> GetPhysicalPublicAgents(AgentsProfileListRequest request)
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
                         SellingProducts = (from hp in _context.HubAgentProducts.Where(s => s.AgentId == user.id)
                                            join i in _context.ProductImages on hp.Id equals i.ProductId
                                            join p in _context.HubProducts on hp.ProductId equals p.Id
                                            select new { p, hp, i }).GroupBy(z => z.p.Name)
                                            .Select(d => new SellingProduct
                                            {
                                                Name = d.Key,
                                                Image = d.Select(e => e.i.ImageUrl).FirstOrDefault()
                                            }).Take(10).ToList(),
                         ReviewCount = (from hp in _context.HubAgentProducts.Where(s => s.AgentId == user.id)
                                        join r in _context.HubReviews on hp.Id equals r.ProductId
                                        where r.IsDigital == false
                                        select r).Select(r => r.ProductId).Count(),
                         MaxRating = (from hp in _context.HubAgentProducts.Where(s => s.AgentId == user.id)
                                      join r in _context.HubReviews on hp.Id equals r.ProductId
                                      where r.IsDigital == false
                                      select r.Rating).OrderByDescending(r => r).FirstOrDefault(),
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
            uquery = uquery.Where(e => e.SellingProducts.Any(r => r.Name!.Contains(request.ProductName)));
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

    public IQueryable<ProductDetailsResponse> GetPublicAgentProducts(int categoryId, int agentId)
    {
        var query = (from user in _context.userstb
                     where user.IsAgent == true && user.IsPublicAgent == true && user.id == agentId
                     join ph in _context.HubAgentProducts on user.id equals ph.AgentId
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

    public IQueryable<HubFAQResponse> GetFAQs()
    {
        var query = _context.HubFAQs.Select(r => new HubFAQResponse
        {
            Answer = r.Answer,
            Question = r.Question
        });
        return query;
    }

    public async Task<HubAgentProduct?> GetProduct(long productId)
    {
        return await _context.HubAgentProducts.FirstOrDefaultAsync(e => e.Id == productId);
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

    public async Task<ProductMetricResponse> GetProductMetrics(int agentId)
    {
        var physicalProduct = await _context.HubAgentProducts.Where(e => e.AgentId == agentId).CountAsync();
        var digitalProduct = await _context.HubDigitalProducts.Where(e => e.AgentId == agentId).CountAsync();
        var metrics = new ProductMetricResponse
        {
            TotalPhysicalProduct = physicalProduct,
            TotalDigitalProductCount = digitalProduct,
            TotalProduct = physicalProduct + digitalProduct
        };
        return metrics;
    }

    public async Task<DashboardMetricResponse> GetDashboardMetrics(int agentId, string userEmail)
    {
        var walletBalance = await _context.wallettb.Where(e => e.UserId == userEmail).Select(c => c.Balance).FirstOrDefaultAsync();
        var physicalProduct = await _context.HubAgentProducts.Where(e => e.AgentId == agentId).CountAsync();
        var digitalProduct = await _context.HubDigitalProducts.Where(e => e.AgentId == agentId).CountAsync();
        var requestCount = await _context.HubProductRequests.Where(e => e.AgentId == agentId).CountAsync();
        var metrics = new DashboardMetricResponse
        {
            Earning = walletBalance,
            TotalProductCount = physicalProduct + digitalProduct,
            TotalRequestCount = requestCount
        };

        var query = from item in _context.HubOrderItems
                    where item.AgentId == agentId
                    join order in _context.HubOrders on item.OrderCode equals order.Code
                    select new { order };
        var orderData = new OrderDataResponse();
        if (query.Any())
        {
            var queryD = query.GroupBy(d => d.order.Code);
            orderData = new OrderDataResponse
            {
                TotalOrderCount = queryD.Select(d => d.Key).Count(),
                TotalPendingCount = queryD.Where(r => r.Select(e => e.order.Status).FirstOrDefault() == OrderStatus.Order).Count(),
                TotalFulfillCount = queryD.Where(r => r.Select(e => e.order.Status).FirstOrDefault() == OrderStatus.Completed).Count()
            };
        }
        metrics.OrderData = orderData;

        return metrics;
    }

    public async Task DeleteProduct(long productId, bool isDigital, int agentId)
    {
        if (isDigital)
        {
            await _context.HubDigitalProducts.Where(x => x.Id == productId && x.AgentId == agentId).ExecuteDeleteAsync();
        }
        else
        {
            await _context.HubAgentProducts.Where(x => x.Id == productId && x.AgentId == agentId).ExecuteDeleteAsync();
        }
    }
    #endregion
}