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
using static DaradsHubAPI.Domain.Enums.Enum;

namespace DaradsHubAPI.Core.Services.Concrete;
public class OrderService(IUnitOfWork _unitOfWork, IServiceProvider _serviceProvider) : IOrderService
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
                var newOrderItem = new HubOrder
                {
                    ShippingAddressId = request.ShippingAddressId,
                    UserEmail = email,
                    OrderDate = GetLocalDateTime.CurrentDateTime(),
                    TotalCost = totalCost,
                    Code = orderCode,
                    Status = OrderStatus.Order,
                    AgentId = productInfo.AgentId,
                    ProductType = "Physical",
                    DeliveryMethodType = request.DeliveryMethodType
                };

                await _unitOfWork.Orders.Insert(newOrderItem);
                productInfo.Stock -= product.Quantity;

                customerWallet.Balance -= totalCost;
                customerWallet.UpdateDate = GetLocalDateTime.CurrentDateTime();
                await _unitOfWork.Orders.SaveAsync();
                if (!string.IsNullOrEmpty(productInfo.Caption))
                    productNames.Add(productInfo.Caption);

                var refNo = string.Concat("wallet-debit", "-", CustomizeCodes.ReferenceCode().AsSpan(0, 5));
                var walletTransaction = new GwalletTran
                {
                    DR = totalCost,
                    orderid = Convert.ToInt32(newOrderItem.Id),
                    walletBal = customerWallet.Balance.GetValueOrDefault(),
                    amt = totalCost,
                    userName = email,
                    refNo = refNo,
                    transMedium = "Wallet",
                    transdate = GetLocalDateTime.CurrentDateTime(),
                    transStatus = "D",
                    transType = "DebitWallet",
                    Status = "Complete",
                    areaCode = newOrderItem.Id.ToString(),
                    orderItem = $"{productInfo.Caption}"
                };
                await _unitOfWork.WalletTransactions.Insert(walletTransaction);
            }
        }

        var track = new HubOrderTracking
        {
            DateCreated = GetLocalDateTime.CurrentDateTime(),
            Status = OrderStatus.Order,
            Description = GenericStrings.ORDERDESCRIPTION,
            OrderCode = orderCode
        };

        await _unitOfWork.Orders.AddHubOrderTracking(track);
        var products = string.Join(',', productNames);

        var scope = _serviceProvider.GetRequiredService<IEmailService>();
        string smessage = $"Hello {email}! Thank you for purchasing {products} from Darads. Your order {orderCode} has been confirmed. We will provide a tracking link once your order has shipped.";
        scope.SendMail(email!, "Product purchased", smessage, "Darads");

        foreach (var prod in request.ProductDetails)
        {
            await _unitOfWork.Orders.DeleteCart(userId, prod.ProductId);
        }
        return new ApiResponse<string> { Status = true, Message = $"Product(s) has been purchased successfully.", StatusCode = StatusEnum.Success, Data = orderCode };
    }

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