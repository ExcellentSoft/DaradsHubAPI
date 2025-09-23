using DaradsHubAPI.Core.IRepository;
using DaradsHubAPI.Core.Model.Request;
using DaradsHubAPI.Core.Model.Response;
using DaradsHubAPI.Domain.Entities;
using DaradsHubAPI.Infrastructure;
using DaradsHubAPI.Shared.Static;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections;
using System.Security.Cryptography;
using System.Threading.Tasks;
using static DaradsHubAPI.Domain.Enums.Enum;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace DaradsHubAPI.Core.Repository
{
    public class OrderRepository(AppDbContext _context) : GenericRepository<HubOrder>(_context), IOrderRepository
    {
        #region Admin Queries
        public async Task<IEnumerable<DailySalesOverviewResponse>> DailySalesOverview()
        {
            var query = from item in _context.HubOrderItems
                        join order in _context.HubOrders on item.OrderCode equals order.Code
                        join agent in _context.userstb on item.AgentId equals agent.id
                        group new { order, item } by new
                        {
                            agent.id,
                            agent.fullname
                        } into g
                        select new DailySalesOverviewResponse
                        {
                            AgentId = g.Key.id,
                            AgentName = g.Key.fullname,
                            // Revenue = Sum of item.Price * item.Quantity
                            Revenue = g.Sum(x => x.item.Price * x.item.Quantity),
                            // Orders = number of unique order codes
                            Orders = g.Select(x => x.order.Code).Distinct().Count()
                        };

            return await query.ToListAsync();
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