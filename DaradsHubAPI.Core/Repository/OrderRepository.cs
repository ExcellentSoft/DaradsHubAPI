using DaradsHubAPI.Core.IRepository;
using DaradsHubAPI.Core.Model;
using DaradsHubAPI.Core.Model.Request;
using DaradsHubAPI.Core.Model.Response;
using DaradsHubAPI.Domain.Entities;
using DaradsHubAPI.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using static DaradsHubAPI.Domain.Enums.Enum;

namespace DaradsHubAPI.Core.Repository
{
    public class OrderRepository(AppDbContext _context) : GenericRepository<HubOrder>(_context), IOrderRepository
    {
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
                                                                        ProductImage = _context.DigitalProductImages.Where(d => d.ProductId == item.ProductId).Select(d => d.ImageUrl).FirstOrDefault()
                                                                    }).FirstOrDefault() :
                                                       (from item in _context.HubOrderItems.Where(d => d.OrderCode == order.Code)
                                                        join product in _context.HubAgentProducts on item.ProductId equals product.Id
                                                        select new ProductData
                                                        {
                                                            ProductName = product.Caption,
                                                            ProductImage = _context.DigitalProductImages.Where(d => d.ProductId == item.ProductId).Select(d => d.ImageUrl).FirstOrDefault()
                                                        }).FirstOrDefault(),

                    AgentName = _context.userstb.Where(d => d.id == order.AgentId && d.IsAgent.GetValueOrDefault() == true).Select(e => e.fullname).FirstOrDefault()
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
    }
}