using DaradsHubAPI.Core.IRepository;
using DaradsHubAPI.Core.Model.Request;
using DaradsHubAPI.Core.Model.Response;
using DaradsHubAPI.Domain.Entities;
using DaradsHubAPI.Infrastructure;
using DaradsHubAPI.Shared.Customs;
using DaradsHubAPI.Shared.Static;
using Microsoft.EntityFrameworkCore;
using static DaradsHubAPI.Domain.Enums.Enum;

namespace DaradsHubAPI.Core.Repository
{
    public class OrderRepository(AppDbContext _context) : GenericRepository<HubOrder>(_context), IOrderRepository
    {
        #region Admin Queries
        public async Task<List<LastFourCustomerRequest>> GetLastCustomerRequests()
        {
            var response = await (from req in _context.HubProductRequests
                                  where req.Status == RequestStatus.Pending
                                  orderby req.DateCreated descending
                                  select new LastFourCustomerRequest
                                  {
                                      DateCreated = req.DateCreated,
                                      ProductService = req.CustomerNeed,
                                      Location = req.Location,
                                      PreferredDate = req.PreferredDate,
                                      Quantity = req.Quantity,
                                      Reference = $"#REQ-{req.Id}",
                                      RequestId = req.Id,
                                      Customer = _context.userstb.Where(r => r.id == req.CustomerId).Select(e => new RequestedUser
                                      {
                                          FullName = e.fullname,
                                          Photo = e.Photo
                                      }).FirstOrDefault(),
                                      Status = req.Status,
                                  }).Take(4).ToListAsync();
            return response;
        }

        public async Task<IEnumerable<TopPerformingAgentResponse>> TopPerformingAgents2()
        {
            var today = DateTime.Now.Date;
            var yesterday = today.AddDays(-1);

            var query = from item in _context.HubOrderItems
                        join order in _context.HubOrders on item.OrderCode equals order.Code
                        join agent in _context.userstb on item.AgentId equals agent.id
                        where order.OrderDate.Date == today || order.OrderDate.Date == yesterday
                        select new
                        {
                            AgentId = agent.id,
                            AgentName = agent.fullname,
                            agent.Photo,
                            OrderDate = order.OrderDate.Date,
                            Amount = item.Price * item.Quantity,
                            order
                        };

            var agentTrend = await query
                .GroupBy(x => new { x.AgentId, x.AgentName, x.OrderDate })
                .Select(g => new
                {
                    g.Key.AgentId,
                    g.Key.AgentName,
                    g.Key.OrderDate,
                    Revenue = g.Sum(x => x.Amount),
                    Orders = g.Select(x => x.order.Code).Distinct().Count(),
                    Photo = g.Select(r => r.Photo).FirstOrDefault()
                }).OrderByDescending(w => w.Revenue)
                .ToListAsync();

            var result = agentTrend
                .GroupBy(a => new { a.AgentId, a.AgentName })
                .Select(g => new TopPerformingAgentResponse
                {
                    AgentId = g.Key.AgentId,
                    AgentName = g.Key.AgentName,
                    OrdersCount = g.Select(d => d.Orders).LastOrDefault(),
                    Photo = g.Select(d => d.Photo).FirstOrDefault(),
                    Revenue = g.Where(d => d.OrderDate == today).Sum(d => d.Revenue),
                    TrendPercentage = g.Where(d => d.OrderDate == yesterday).Sum(d => d.Revenue) > 0
                        ? Math.Round(((g.Where(d => d.OrderDate == today).Sum(d => d.Revenue) -
                                      g.Where(d => d.OrderDate == yesterday).Sum(d => d.Revenue)) /
                                      g.Where(d => d.OrderDate == yesterday).Sum(d => d.Revenue)) * 100, 2)
                        : 100
                })
                .Take(4)
                .ToList();

            return result;

        }

        public async Task<IEnumerable<TopPerformingAgentResponse>> TopPerformingAgents()
        {
            var today = DateTime.UtcNow.Date;
            var yesterday = today.AddDays(-1);

            var query = await (from item in _context.HubOrderItems
                               join order in _context.HubOrders on item.OrderCode equals order.Code
                               join agent in _context.userstb on item.AgentId equals agent.id
                               where order.OrderDate.Date == today || order.OrderDate.Date == yesterday
                               group new { item, order } by new { agent.fullname, agent.Photo, agent.id } into g
                               orderby g.Sum(x => x.item.Quantity) descending
                               select new TopPerformingAgentResponse
                               {
                                   AgentId = g.Key.id,
                                   AgentName = g.Key.fullname,
                                   Photo = g.Key.Photo,
                                   Revenue = g.Sum(x => x.item.Price * x.item.Quantity),
                                   OrdersCount = g.Select(x => x.order.Code).Distinct().Count(),
                                   TrendPercentage = g.Where(d => d.order.OrderDate == yesterday).Sum(x => x.item.Price * x.item.Quantity) > 0
            ? Math.Round(((g.Where(d => d.order.OrderDate == today).Sum(x => x.item.Price * x.item.Quantity) -
                          g.Where(d => d.order.OrderDate == yesterday).Sum(x => x.item.Price * x.item.Quantity)) /
                          g.Where(d => d.order.OrderDate == yesterday).Sum(x => x.item.Price * x.item.Quantity)) * 100, 2) : 100

                               }).Take(10).ToListAsync();
            return query;
        }

        public async Task<DailySalesOverviewResponse?> DailySalesOverview()
        {
            var today = GetLocalDateTime.CurrentDateTime();
            var query = from item in _context.HubOrderItems
                        join order in _context.HubOrders on item.OrderCode equals order.Code
                        join agent in _context.userstb on item.AgentId equals agent.id
                        where order.OrderDate.Date == today.Date
                        select new { item, order, agent };

            var response = new DailySalesOverviewResponse
            {

                DailySalesOverviews = await query.GroupBy(r => new { r.agent.id, r.agent.fullname }).Select(e => new DailySalesOverview
                {
                    AgentId = e.Key.id,
                    AgentName = e.Key.fullname,
                    // Revenue = Sum of item.Price * item.Quantity
                    Revenue = e.Sum(x => x.item.Price * x.item.Quantity),
                    // Orders = number of unique order codes
                    Orders = e.Select(x => x.order.Code).Distinct().Count()
                }).ToListAsync(),
                Date = await query.Select(e => e.order.OrderDate).FirstOrDefaultAsync(),
                TotalOrderAmount = await query.SumAsync(x => x.item.Price * x.item.Quantity)
            };
            return response;
        }

        public async Task<AdminDashboardMetricResponse> AdminDashboardMetrics()
        {
            var metrics = new AdminDashboardMetricResponse();

            var totalWallet = await (from user in _context.userstb
                                     where user.IsAgent == true
                                     join wallet in _context.wallettb on user.email equals wallet.UserId
                                     select wallet).SumAsync(e => e.Balance);

            var totalDigitalProduct = await (from user in _context.userstb
                                             where user.IsAgent == true
                                             join dp in _context.HubDigitalProducts on user.id equals dp.AgentId
                                             select dp).CountAsync();

            var totalPhysicalProduct = await (from user in _context.userstb
                                              where user.IsAgent == true
                                              join dp in _context.HubAgentProducts on user.id equals dp.AgentId
                                              select dp).CountAsync();

            var totalWithdrawn = await _context.HubWithdrawalRequests
                .Where(e => e.Status == WithdrawalRequestStatus.Paid)
                .Select(w => w.Amount).SumAsync();

            var orderQuery = from item in _context.HubOrderItems
                             join order in _context.HubOrders on item.OrderCode equals order.Code
                             join agent in _context.userstb on item.AgentId equals agent.id
                             select new { item, order, agent };
            var today = GetLocalDateTime.CurrentDateTime();
            var orderdata = new OrderData
            {
                TodayOrderAmount = await orderQuery.Where(d => d.order.OrderDate.Date == today.Date).SumAsync(x => x.item.Price * x.item.Quantity),
                TodayOrdersCount = await orderQuery.Where(d => d.order.OrderDate.Date == today.Date).Select(x => x.order.Code).Distinct().CountAsync()
            };

            var yesterdayOrderAmount = await orderQuery.Where(d => d.order.OrderDate.Date == today.AddDays(-1).Date).SumAsync(x => x.item.Price * x.item.Quantity);

            var different = yesterdayOrderAmount > 0
        ? Math.Round(((orderdata.TodayOrderAmount - yesterdayOrderAmount) / yesterdayOrderAmount) * 100, 0) : 100;// default if no data yesterday

            orderdata.PercentageChange = different;
            metrics.TotalWithdrawn = totalWithdrawn;
            metrics.AgentRevenueBalance = totalWallet;
            metrics.ProductsCount = totalDigitalProduct + totalPhysicalProduct;
            metrics.OrderData = orderdata;


            return metrics;
        }
        #endregion

        public IQueryable<OrderListResponse> GetOrders(string email, OrderListRequest request)
        {
            var qOrder = from order in _context.HubOrders.Where(e => e.UserEmail == email)
                         where (request.StartDate == null || order.OrderDate.Date >= request.StartDate.Value.Date) &&
                              (request.EndDate == null || order.OrderDate.Date <= request.EndDate.Value.Date)
                         orderby order.OrderDate descending
                         select order;
            if (qOrder.Any())
            {
                if (!string.IsNullOrEmpty(request.SearchText))
                {
                    var searchText = request.SearchText.Trim().ToLower();

                    qOrder = qOrder.Where(d => d.Code.Contains(request.SearchText)).OrderByDescending(d => d.OrderDate);
                }

                var query = qOrder.Select(order => new OrderListResponse
                {
                    OrderDate = order.OrderDate,
                    Code = order.Code,
                    OrderStatus = order.Status,
                    ProductType = order.ProductType,
                    TotalPrice = order.TotalCost,
                    ProductData = order.ProductType == "Digital" ? (from item in _context.HubOrderItems.Where(d => d.OrderCode == order.Code)
                                                                    join product in _context.HubDigitalProducts on item.ProductId equals product.Id
                                                                    join cat in _context.Catalogues on product.CatalogueId equals cat.Id
                                                                    select new ProductData
                                                                    {
                                                                        ProductName = cat.Name,
                                                                        ProductImage = _context.DigitalProductImages.Where(d => d.ProductId == item.ProductId).Select(d => d.ImageUrl).FirstOrDefault(),
                                                                        AgentName = _context.userstb.Where(d => d.id == item.AgentId).Select(d => d.fullname).FirstOrDefault()
                                                                    }).FirstOrDefault() :
                                                       (from item in _context.HubOrderItems.Where(d => d.OrderCode == order.Code)
                                                        join product in _context.HubAgentProducts on item.ProductId equals product.Id
                                                        select new ProductData
                                                        {
                                                            ProductName = product.Caption,
                                                            ProductImage = _context.DigitalProductImages.Where(d => d.ProductId == item.ProductId).Select(d => d.ImageUrl).FirstOrDefault(),
                                                            AgentName = _context.userstb.Where(d => d.id == item.AgentId).Select(d => d.fullname).FirstOrDefault()
                                                        }).FirstOrDefault()
                });

                return query;
            }
            return Enumerable.Empty<OrderListResponse>().AsQueryable();
        }

        public IQueryable<CartResponse> GetCartsListByUserId(int userId)
        {
            var carts = from cat in _context.shopCats
                        join pro in _context.HubAgentProducts on cat.ProductId equals pro.Id
                        join user in _context.userstb on pro.AgentId equals user.id
                        join p in _context.HubProducts on pro.ProductId equals p.Id
                        where cat.userId == userId
                        select new CartResponse
                        {
                            Id = cat.id,
                            Description = pro.Description,
                            AgentName = user.fullname,
                            Price = pro.Price,
                            ProductId = pro.Id,
                            ProductName = p.Name ?? pro.Caption ?? "",
                            Quantity = cat.Quantity,
                            SubTotal = pro.Price * cat.Quantity,
                            UserId = userId,
                            ImageUrl = _context.ProductImages.Where(i => i.ProductId == pro.Id).Select(f => f.ImageUrl).FirstOrDefault() ?? ""
                        };
            return carts;
        }

        public async Task<shopCat?> GetCart(int userId, long productId)
        {
            var cart = await _context.shopCats.FirstOrDefaultAsync(d => d.userId == userId && d.ProductId == productId);
            return cart;
        }

        public async Task<HubOrderTracking?> GetOrderTracking(string orderCode)
        {
            var tracking = await _context.HubOrderTracking.FirstOrDefaultAsync(d => d.OrderCode == orderCode);
            return tracking;
        }

        public async Task DeleteCart(int userId, long productId)
        {
            var cart = await _context.shopCats.FirstOrDefaultAsync(d => d.userId == userId && d.ProductId == productId);

            if (cart is not null)
            {
                _context.shopCats.Remove(cart);
                await _context.SaveChangesAsync();
            }
        }
        public async Task DeleteShippingAddress(int userId, long addressId)
        {
            var address = await _context.ShippingAddresses.FirstOrDefaultAsync(d => d.Id == addressId && d.CustomerId == userId);

            if (address is not null)
            {
                _context.ShippingAddresses.Remove(address);
                await _context.SaveChangesAsync();
            }
        }
        public async Task UpdateCart(shopCat model)
        {
            _context.shopCats.Update(model);
            await _context.SaveChangesAsync();
        }
        public async Task AddCart(shopCat model)
        {
            _context.shopCats.Add(model);
            await _context.SaveChangesAsync();
        }
        public void AddOrderItem(HubOrderItem model)
        {
            _context.HubOrderItems.Add(model);
        }
        public async Task AddHubOrderTracking(HubOrderTracking model)
        {
            _context.HubOrderTracking.Add(model);
            await _context.SaveChangesAsync();
        }
        public async Task AddShippingAddress(ShippingAddress model)
        {
            _context.ShippingAddresses.Add(model);
            await _context.SaveChangesAsync();
        }
        public IQueryable<ShippingAddress> GetShippingAddresses(int userId)
        {
            var shippingAddress = _context.ShippingAddresses.Where(d => d.CustomerId == userId);
            return shippingAddress;
        }

        public async Task<AgentOrderMetricResponse> GetOrderMetrics(int agentId)
        {
            var query = from item in _context.HubOrderItems
                        where item.AgentId == agentId
                        join order in _context.HubOrders on item.OrderCode equals order.Code
                        select new { order };
            var metrics = new AgentOrderMetricResponse();
            if (query.Any())
            {
                var queryD = query.GroupBy(d => d.order.Code);
                metrics = new AgentOrderMetricResponse
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

        public async Task<CatalogueInsightResponse> GetCatalogueInsight(int agentId)
        {
            var insight = new CatalogueInsightResponse();
            insight.TotalDigitalProductCount = await _context.HubDigitalProducts.Where(d => d.AgentId == agentId).CountAsync();

            var bestSeller = await (from item in _context.HubOrderItems
                                    where item.AgentId == agentId
                                    join order in _context.HubOrders on item.OrderCode equals order.Code
                                    where order.ProductType == "Digital" && order.Status == OrderStatus.Completed
                                    join dp in _context.HubDigitalProducts on item.ProductId equals dp.Id
                                    group new { item, dp } by new { dp.Id, dp.Title, dp.Price } into g
                                    orderby g.Sum(x => x.item.Quantity) descending
                                    select new BestSeller
                                    {
                                        Name = g.Key.Title,
                                        TotalSales = g.Sum(x => x.item.Quantity)
                                    }).FirstOrDefaultAsync();
            insight.BestSeller = bestSeller;

            return await Task.FromResult(insight);
        }

        public async Task<List<AgentOrderListResponse>> GetAgentOrders(AgentOrderListRequest request, int agentId)
        {
            var qOrder = from item in _context.HubOrderItems
                         where item.AgentId == agentId
                         join order in _context.HubOrders on item.OrderCode equals order.Code
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

        public async Task<SingleOrderResponse?> GetAgentOrder(string orderCode)
        {
            var response = await (from order in _context.HubOrders
                                  where order.Code == orderCode
                                  select new SingleOrderResponse
                                  {
                                      Code = order.Code,
                                      OrderDate = order.OrderDate,
                                      OrderId = order.Id,
                                      OrderStatus = order.Status,
                                      DeliveryMethod = order.DeliveryMethodType.GetDescription(),
                                      TotalPrice = order.TotalCost,
                                      ProductDetails = order.ProductType == "Digital" ? (from item in _context.HubOrderItems.Where(d => d.OrderCode == order.Code)
                                                                                         join product in _context.HubDigitalProducts on item.ProductId equals product.Id
                                                                                         join cat in _context.Catalogues on product.CatalogueId equals cat.Id
                                                                                         select new OrderProductRecord
                                                                                         {

                                                                                             Name = cat.Name,
                                                                                             Price = item.Price,
                                                                                             Quantity = item.Quantity

                                                                                         }).ToList() :
                                                         (from item in _context.HubOrderItems.Where(d => d.OrderCode == order.Code)
                                                          join product in _context.HubAgentProducts on item.ProductId equals product.Id
                                                          select new OrderProductRecord
                                                          {

                                                              Name = product.Caption ?? "Nil",
                                                              Price = item.Price,
                                                              Quantity = item.Quantity

                                                          }).ToList(),

                                      CustomerOrderRecord = (from user in _context.userstb
                                                             where order.UserEmail == user.email
                                                             from address in _context.ShippingAddresses
                                                             where address.Id == order.ShippingAddressId
                                                             select new CustomerOrderRecord
                                                             {
                                                                 Address = address.Address,
                                                                 City = address.City,
                                                                 Email = user.email,
                                                                 PhoneNumber = user.phone
                                                             }).FirstOrDefault(),
                                      OrderActivitiesRecords = (from track in _context.HubOrderTracking
                                                                where track.OrderCode == order.Code
                                                                select new OrderActivitiesRecord
                                                                {
                                                                    DateCreated = track.DateCreated,
                                                                    Description = track.Description,
                                                                    Status = track.Status
                                                                }).ToList()
                                  }).FirstOrDefaultAsync();


            return response;
        }
    }
}