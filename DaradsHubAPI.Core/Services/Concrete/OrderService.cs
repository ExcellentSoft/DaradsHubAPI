using DaradsHubAPI.Core.IRepository;
using DaradsHubAPI.Core.Model;
using DaradsHubAPI.Core.Model.Request;
using DaradsHubAPI.Core.Model.Response;
using DaradsHubAPI.Core.Services.Interface;
using DaradsHubAPI.Domain.Entities;
using DaradsHubAPI.Shared.Customs;
using DaradsHubAPI.Shared.Static;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using DaradsHubAPI.Domain.Enums;
using static DaradsHubAPI.Domain.Enums.Enum;

namespace DaradsHubAPI.Core.Services.Concrete;
public class OrderService(IUnitOfWork _unitOfWork, IServiceProvider _serviceProvider, IEmailService _emailService) : IOrderService
{

    public async Task<ApiResponse<IEnumerable<OrderListResponse>>> GetOrders(string email, OrderListRequest request)
    {
        var query = _unitOfWork.Orders.GetOrders(email, request);

        var totalOrders = query.Count();
        var paginatedOrders = await Task.FromResult(query.Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize).ToList());

        return new ApiResponse<IEnumerable<OrderListResponse>> { Message = "Orders fetched successfully.", Status = true, Data = paginatedOrders, StatusCode = StatusEnum.Success, TotalRecord = totalOrders, Pages = request.PageSize, CurrentPageCount = request.PageNumber };
    }

    public async Task<ApiResponse<IEnumerable<CartResponse>>> GetCart(int userId)
    {
        var cartItems = _unitOfWork.Orders.GetCartsListByUserId(userId);
        if (cartItems == null || !cartItems.Any())
            return new ApiResponse<IEnumerable<CartResponse>> { Status = false, Message = $"Cart items record not found.", StatusCode = StatusEnum.NoRecordFound };

        var cartResponse = await cartItems.ToListAsync();
        return new ApiResponse<IEnumerable<CartResponse>> { Status = true, Message = $"Product added to cart successfully ", StatusCode = StatusEnum.Success, Data = cartResponse };
    }
    public async Task<ApiResponse> AddItemsToCart(AddItemToCartRequestModel request, int userId)
    {
        var productInfo = await _unitOfWork.Products.GetProduct(request.ProductId);
        if (productInfo == null)
            return new ApiResponse("Product not found", StatusEnum.NoRecordFound, false);

        var existingCartItem = await _unitOfWork.Orders.GetCart(userId, request.ProductId);

        if (existingCartItem != null)
        {
            existingCartItem.Quantity += request.Quantity;
            await _unitOfWork.Orders.UpdateCart(existingCartItem);
        }
        else
        {
            var newCartItem = new shopCat
            {
                userId = userId,
                ProductId = request.ProductId,
                Quantity = request.Quantity,
            };
            await _unitOfWork.Orders.AddCart(newCartItem);

        }
        return new ApiResponse("Successful.", StatusEnum.Success, true);
    }
    public async Task<ApiResponse> RemoveItemsFromCart(AddItemToCartRequestModel addToCartRequestModel, int userId)
    {
        var productInfo = await _unitOfWork.Products.GetProduct(addToCartRequestModel.ProductId);
        if (productInfo == null)
            return new ApiResponse("Product not found", StatusEnum.NoRecordFound, false);

        await _unitOfWork.Orders.DeleteCart(userId, addToCartRequestModel.ProductId);
        if (addToCartRequestModel.Quantity >= 1)
        {
            var newCartItem = new shopCat
            {
                userId = userId,
                ProductId = addToCartRequestModel.ProductId,
                Quantity = addToCartRequestModel.Quantity
            };

            await _unitOfWork.Orders.AddCart(newCartItem);
        }
        return new ApiResponse("Successful", StatusEnum.Success, true);
    }

    public async Task<ApiResponse> AddShippingAddress(ShippingAddressRequest request, int userId)
    {
        var shipping = new ShippingAddress
        {
            CustomerId = userId,
            Address = request.Address,
            City = request.City ?? "",
            Country = string.Empty,
            Email = request.Email ?? string.Empty,
            PhoneNumber = request.PhoneNumber,
            State = request.State ?? string.Empty,

        };
        await _unitOfWork.Orders.AddShippingAddress(shipping);
        return new ApiResponse("Successful", StatusEnum.Success, true);
    }
    public async Task<ApiResponse> RemoveShippingAddress(int userId, long addressId)
    {
        await _unitOfWork.Orders.DeleteShippingAddress(userId, addressId);
        return new ApiResponse("Successful", StatusEnum.Success, true);
    }
    public async Task<ApiResponse<IEnumerable<ShippingAddressResponse>>> GetShippingAddress(int userId)
    {
        var query = _unitOfWork.Orders.GetShippingAddresses(userId);
        var response = await query.Select(e => new ShippingAddressResponse
        {
            Address = e.Address,
            City = e.City,
            PhoneNumber = e.PhoneNumber,
            Email = e.Email,
            State = e.State,
            Id = e.Id
        }).ToListAsync();

        return new ApiResponse<IEnumerable<ShippingAddressResponse>> { Status = true, Message = "Successful", StatusCode = StatusEnum.Success, Data = response };
    }
    public async Task<ApiResponse<string>> CheckOut(CheckoutRequest request, string email, int userId)
    {
        var (status, message) = ValidateCheckoutRequest(request);
        if (!status)
        {
            return new ApiResponse<string> { Status = status, Message = message, StatusCode = StatusEnum.Validation };
        }

        var customerWallet = await _unitOfWork.Wallets.GetSingleWhereAsync(cw => cw.UserId == email);
        if (customerWallet is null)
            return new ApiResponse<string> { Status = false, Message = "Customer wallet record not found.", StatusCode = StatusEnum.NoRecordFound };

        if (customerWallet.Balance < request.Price)
            return new ApiResponse<string> { Status = false, Message = "Your wallet balance is insufficient. Please attempt to fund your wallet.", StatusCode = StatusEnum.Validation };

        var (_status, _message) = await ValidateProducts(request);
        if (!_status)
        {
            return new ApiResponse<string> { Status = _status, Message = _message, StatusCode = StatusEnum.Validation };
        }

        decimal totalCost = 0;
        var orderCode = string.Concat($"{CustomizeCodes.GenerateRandomCode(2)}-{CustomizeCodes.GetUniqueId().AsSpan(0, 5)}");
        var productNames = new List<string>();
        foreach (var product in request.ProductDetails)
        {
            var productInfo = await _unitOfWork.Products.GetSingleWhereAsync(x => x.Id == product.ProductId);
            if (productInfo is not null)
            {
                totalCost += productInfo.Price * product.Quantity;
                var newOrderItem = new HubOrderItem
                {
                    OrderCode = orderCode,
                    ProductId = product.ProductId,
                    Quantity = product.Quantity,
                    CreatedDate = GetLocalDateTime.CurrentDateTime(),
                    Price = productInfo.Price,
                    AgentId = productInfo.AgentId
                };
                _unitOfWork.Orders.AddOrderItem(newOrderItem);
                productInfo.Stock -= product.Quantity;

                if (!string.IsNullOrEmpty(productInfo.Caption))
                    productNames.Add(productInfo.Caption);
            }
        }

        customerWallet.Balance -= totalCost;
        customerWallet.UpdateDate = GetLocalDateTime.CurrentDateTime();
        await _unitOfWork.Orders.SaveAsync();

        var newOrder = new HubOrder
        {
            ShippingAddressId = request.ShippingAddressId,
            UserEmail = email,
            OrderDate = GetLocalDateTime.CurrentDateTime(),
            TotalCost = totalCost,
            Code = orderCode,
            Status = OrderStatus.Order,
            ProductType = "Physical",
            DeliveryMethodType = request.DeliveryMethodType
        };
        await _unitOfWork.Orders.Insert(newOrder);
        await _unitOfWork.Orders.SaveAsync();

        var products = string.Join(',', productNames);
        var refNo = string.Concat("wallet-debit", "-", CustomizeCodes.ReferenceCode().AsSpan(0, 5));
        var walletTransaction = new GwalletTran
        {
            DR = totalCost,
            orderid = Convert.ToInt32(newOrder.Id),
            walletBal = customerWallet.Balance.GetValueOrDefault(),
            amt = totalCost,
            userName = email,
            refNo = refNo,
            transMedium = "Wallet",
            transdate = GetLocalDateTime.CurrentDateTime(),
            transStatus = "D",
            transType = "DebitWallet",
            Status = "Complete",
            areaCode = newOrder.Id.ToString(),
            orderItem = products
        };

        await _unitOfWork.WalletTransactions.Insert(walletTransaction);
        var track = new HubOrderTracking
        {
            DateCreated = GetLocalDateTime.CurrentDateTime(),
            Status = OrderStatus.Order,
            Description = GenericStrings.ORDERDESCRIPTION,
            OrderCode = orderCode
        };

        await _unitOfWork.Orders.AddHubOrderTracking(track);


        var scope = _serviceProvider.GetRequiredService<IEmailService>();
        string smessage = $"Hello {email}! Thank you for purchasing {products} from Darads. Your order {orderCode} has been confirmed. We will provide a tracking link once your order has shipped.";
        scope.SendMail(email!, "Product purchased", smessage, "Darads");

        foreach (var prod in request.ProductDetails)
        {
            await _unitOfWork.Orders.DeleteCart(userId, prod.ProductId);
        }
        await _unitOfWork.Notifications.SaveNotification(new HubNotification
        {
            TimeCreated = GetLocalDateTime.CurrentDateTime(),
            Title = "New Order",
            NoteToEmail = email,
            Message = $"Your new order with code {orderCode} was successfully placed.",
            NotificationType = NotificationType.NewOrder
        });
        return new ApiResponse<string> { Status = true, Message = $"Product(s) has been purchased successfully.", StatusCode = StatusEnum.Success, Data = orderCode };
    }
    public async Task<ApiResponse<DigitalCheckoutResponse>> CheckOutDigital(CheckoutDigitalRequest request, string email)
    {
        var product = await _unitOfWork.DigitalProducts.GetSingleWhereAsync(r => r.IsSold == false && r.Id == request.ProductId);
        if (product is null)
        {
            return new ApiResponse<DigitalCheckoutResponse> { Status = false, Message = "Selected product is not available, please try again later.", StatusCode = StatusEnum.NoRecordFound };
        }
        var catalogue = await _unitOfWork.DigitalProducts.GetCatalogue(product.CatalogueId);
        if (catalogue is null)
        {
            return new ApiResponse<DigitalCheckoutResponse> { Status = false, Message = "Invalid catalogue", StatusCode = StatusEnum.NoRecordFound };
        }

        var customerWallet = await _unitOfWork.Wallets.GetSingleWhereAsync(cw => cw.UserId == email);
        if (customerWallet is null)
            return new ApiResponse<DigitalCheckoutResponse> { Status = false, Message = "Customer wallet record not found.", StatusCode = StatusEnum.NoRecordFound };

        if (customerWallet.Balance < request.Price)
            return new ApiResponse<DigitalCheckoutResponse> { Status = false, Message = "Your wallet balance is insufficient. Please attempt to fund your wallet.", StatusCode = StatusEnum.Validation };

        decimal chargeFees = 0;
        decimal totalCost = product.Price + chargeFees;
        var orderCode = string.Concat($"{CustomizeCodes.GenerateRandomCode(2)}-{CustomizeCodes.GetUniqueId().AsSpan(0, 5)}");

        var newOrderItem = new HubOrderItem
        {
            OrderCode = orderCode,
            ProductId = product.Id,
            Quantity = 1,
            CreatedDate = GetLocalDateTime.CurrentDateTime(),
            Price = product.Price,
            AgentId = product.AgentId
        };
        _unitOfWork.Orders.AddOrderItem(newOrderItem);

        customerWallet.Balance -= totalCost;
        customerWallet.UpdateDate = GetLocalDateTime.CurrentDateTime();
        await _unitOfWork.Orders.SaveAsync();

        var newOrder = new HubOrder
        {
            ShippingAddressId = 0,
            UserEmail = email,
            OrderDate = GetLocalDateTime.CurrentDateTime(),
            TotalCost = totalCost,
            Code = orderCode,
            Status = OrderStatus.Order,
            ProductType = "Digital",
            DeliveryMethodType = DeliveryMethodType.Standard
        };
        await _unitOfWork.Orders.Insert(newOrder);
        await _unitOfWork.Orders.SaveAsync();

        var products = catalogue.Name;
        var refNo = string.Concat("wallet-debit", "-", CustomizeCodes.ReferenceCode().AsSpan(0, 5));
        var walletTransaction = new GwalletTran
        {
            DR = totalCost,
            orderid = Convert.ToInt32(newOrder.Id),
            walletBal = customerWallet.Balance.GetValueOrDefault(),
            amt = totalCost,
            userName = email,
            refNo = refNo,
            transMedium = "Wallet",
            transdate = GetLocalDateTime.CurrentDateTime(),
            transStatus = "D",
            transType = "DebitWallet",
            Status = "Complete",
            areaCode = newOrder.Id.ToString(),
            orderItem = products
        };

        product.IsSold = true;
        await _unitOfWork.WalletTransactions.Insert(walletTransaction);

        var response = new DigitalCheckoutResponse
        {
            Value = product.Value,
            OrderCode = orderCode,
        };
        await _unitOfWork.Notifications.SaveNotification(new HubNotification
        {
            TimeCreated = GetLocalDateTime.CurrentDateTime(),
            Title = "New Order",
            NoteToEmail = email,
            Message = $"Your new order with code {orderCode} was successfully placed.",
            NotificationType = NotificationType.NewOrder
        });
        return new ApiResponse<DigitalCheckoutResponse> { Status = true, Message = $"Product(s) has been purchased successfully.", StatusCode = StatusEnum.Success, Data = response };
    }

    #region Agent
    public async Task<ApiResponse<AgentOrderMetricResponse>> OrderMetrics(int agentId)
    {
        var metrics = await _unitOfWork.Orders.GetOrderMetrics(agentId);
        return new ApiResponse<AgentOrderMetricResponse> { StatusCode = StatusEnum.Success, Message = "Metrics fetched successfully.", Status = true, Data = metrics };
    }

    public async Task<ApiResponse<List<AgentOrderListResponse>>> GetOrders(AgentOrderListRequest request, int agentId)
    {
        //var qOrder = from order in _ecommerceDbContext.orders
        //             where (request.StartDate == null || order.CreatedDate.Date >= request.StartDate.Value.Date) &&
        //                  (request.EndDate == null || order.CreatedDate.Date <= request.EndDate.Value.Date)
        //             select order;

        //if (!string.IsNullOrEmpty(request.SearchText))
        //{
        //    var searchText = request.SearchText.Trim().ToLower();

        //    qOrder = qOrder.Where(d => d.ReferenceNumber.Contains(request.SearchText));
        //}

        //if (request.Status is not null)
        //{
        //    qOrder = qOrder.Where(d => d.Status == request.Status);
        //}

        //var totalRecordsCount = qOrder.Count();
        //var response = await qOrder.Skip((request!.PageNumber - 1) * request.PageSize).Take(request.PageSize).OrderByDescending(d => d.CreatedDate).Select(s => new OrderListResponse
        //{
        //    OrderId = s.Id,
        //    OrderStatus = s.Status,
        //    OrderStatusText = s.Status.GetDescription(),
        //    PurchaseDate = s.CreatedDate,
        //    ReferenceId = s.ReferenceNumber,
        //    ShopperName = s.UserName,
        //    productName = (from item in _ecommerceDbContext.orderitems.Where(d => d.ReferenceNumber == s.ReferenceNumber)
        //                   join product in _ecommerceDbContext.ecommerceproducts on item.ProductId equals product.Id
        //                   select product.Name).FirstOrDefault(),
        //    TotalPrice = s.TotalSum,
        //    TotalProductCount = _ecommerceDbContext.orderitems.Where(d => d.ReferenceNumber == s.ReferenceNumber).Count()
        //}).ToListAsync();

        return new ApiResponse<List<AgentOrderListResponse>> { StatusCode = StatusEnum.Success, Message = "Orders fetched successfully.", Status = true,/* Data = response,*/ Pages = request.PageSize, /*TotalRecord = totalRecordsCount,*/ CurrentPage = request.PageNumber };
    }
    public async Task<ApiResponse<SingleOrderResponse>> GetOrder(string orderCode)
    {
        //var qOrder = await (from order in _ecommerceDbContext.orders.Where(b => b.ReferenceNumber == referenceId)
        //                    join item in _ecommerceDbContext.orderitems on order.ReferenceNumber equals item.ReferenceNumber
        //                    join prod in _ecommerceDbContext.ecommerceproducts on item.ProductId equals prod.Id
        //                    select new { order, item, prod }).GroupBy(d => d.item.ReferenceNumber).Select(d => new
        //                    SingleOrderResponse
        //                    {
        //                        Description = d.Select(s => s.order.Description).FirstOrDefault(),
        //                        ReferenceId = d.Select(s => s.order.ReferenceNumber).FirstOrDefault() ?? "",
        //                        OrderStatus = d.Select(s => s.order.Status).FirstOrDefault(),
        //                        OrderStatusText = d.Select(s => s.order.Status).FirstOrDefault().GetDescription(),
        //                        PurchaseDate = d.Select(s => s.order.CreatedDate).FirstOrDefault(),
        //                        ShopperName = d.Select(s => s.order.UserName).FirstOrDefault() ?? "",
        //                        TotalPrice = d.Select(s => s.order.TotalSum).FirstOrDefault(),
        //                        ProductDetails = d.Select(s => new OrderProductDetails
        //                        {
        //                            Name = s.prod.Name ?? "",
        //                            Price = s.item.Price,
        //                            Quantity = s.item.Quantity,
        //                            TotalPrice = s.item.Quantity * s.item.Price
        //                        }).ToList()

        //                    }).FirstOrDefaultAsync();

        return new ApiResponse<SingleOrderResponse> { StatusCode = StatusEnum.Success, Message = "Order product fetched successfully.", Status = true,/* Data = qOrder ?? new SingleOrderResponse { } */};

    }
    public async Task<ApiResponse> ChangeOrderStatus(ChangeStatusRequest request)
    {
        var order = await _unitOfWork.Orders.GetSingleWhereAsync(e => e.Code == request.OrderCode);
        if (order is null)
        {
            return new ApiResponse("Order record not found.", StatusEnum.NoRecordFound, false);
        }
        var tDescription = GenericStrings.GetDescription(request.Status, "");

        var newAudit = new HubOrderTracking
        {

            Status = request.Status,
            OrderCode = request.OrderCode,
            DateCreated = GetLocalDateTime.CurrentDateTime(),
            Description = tDescription,
        };
        await _unitOfWork.Orders.AddHubOrderTracking(newAudit);

        var newNotification = new HubNotification
        {
            TimeCreated = GetLocalDateTime.CurrentDateTime(),
            Title = "Change Order Status",
            NoteToEmail = order.UserEmail,
            Message = tDescription,
            NotificationType = NotificationType.ChangeOrderStatus
        };

        order.Status = request.Status;
        await _unitOfWork.Notifications.Insert(newNotification);

        string body = $"Hello {order.UserEmail},<br/> Your order with code #{order.Code} has been updated to status: {request.Status.GetDescription()}<br/><br/>";
        _emailService.SendMail(order.UserEmail ?? "", "Change Order status", body, "Darad");

        return new ApiResponse("Success.", StatusEnum.Success, true);
    }

    #endregion
    private async Task<(bool status, string message)> ValidateProducts(CheckoutRequest request)
    {
        var response = (false, "Invalid request.");
        foreach (var product in request.ProductDetails)
        {
            var productInfo = await _unitOfWork.Products.GetSingleWhereAsync(x => x.Id == product.ProductId);
            if (productInfo == null)
            {
                response = new(false, $"Product {product.ProductName} record not found. Try again later.");
            }
            else if (productInfo.Stock < product.Quantity)
            {
                response = new(false, $"Insufficient stock for product {product.ProductName}");
            }
            else
            {
                response = new(true, "Validation passed.");
            }
        }
        return response;
    }

    private static (bool status, string message) ValidateCheckoutRequest(CheckoutRequest request)
    {
        if (!request.ProductDetails.Any())
        {
            return new(false, "Select product(s) and try again later");
        }
        else if (request.ShippingAddressId == 0)
        {
            return new(false, "Shipping address is required.");
        }
        else if (request.Price == 0)
        {
            return new(false, "Product(s) price is required.");
        }
        else if (request.ProductDetails.Any(p => p.Quantity == 0))
        {
            return new(false, "Quantity of product(s) is required.");
        }
        else
        {
            return new(true, "Validation passed.");
        }

    }
}