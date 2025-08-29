using System.ComponentModel.DataAnnotations;
using static DaradsHubAPI.Domain.Enums.Enum;

namespace DaradsHubAPI.Domain.Entities;
#nullable disable
public class RequestLog
{
    [Key]
    public int Id { get; set; }
    public string UserId { get; set; }
    [MaxLength(200)]
    public string LogName { get; set; }
    [MaxLength(500)]
    public string Description { get; set; }
    [MaxLength(15)]
    public string WhatsAppNumber { get; set; }
    [MaxLength(75)]
    public string Email { get; set; }
    public LogRequestEnum Status { get; set; }
    public DateTime TimeCreated { get; set; }
    public DateTime TimeUpdated { get; set; }
    public int Quantity { get; set; }
}

public sealed record RequestLogRecord
{
    public string UserId { get; set; }
    public string LogName { get; set; }
    public string Description { get; set; }
    public string WhatsAppNumber { get; set; }
    public string Email { get; set; }
    public int Quantity { get; set; }
}

public class VendorUploadRequestLog
{
    public VendorUploadRequestLog()
    {
        // Calculate SalesPrice as 30% of Price plus Price
        SalesPrice = Price * 1.3m;
    }
    [Key]
    public int Id { get; set; }
    public string UserId { get; set; }
    public int RequestLogId { get; set; }
    [MaxLength(500)]
    public string LogProfile { get; set; }
    [MaxLength(800)]
    public string LogValue { get; set; }
    [MaxLength(200)]
    public string LogLink { get; set; }
    [MaxLength(750)]
    public string Description { get; set; }
    public UploadRequestEnum Status { get; set; }
    public decimal Price { get; set; }
    public decimal SalesPrice { get; set; }
    public DateTime TimeCreated { get; set; }
    public DateTime TimeUpdated { get; set; }
    public string OrderByCustomer { get; set; }
    public void SetPrice(decimal price)
    {
        Price = price;
        // Recalculate SalesPrice as 30% of new Price plus new Price
        SalesPrice = Price * 1.3m;
    }
}
public sealed record VendorUploadRequestLogRecord
{
    public string UserId { get; set; }
    public int RequestLogId { get; set; }
    public string LogProfile { get; set; }
    public string LogValue { get; set; }
    public string LogLink { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
}

public sealed record UploadRequestLogRecord
{
    public int Id { get; set; }
    public string LogProfile { get; set; }
    public string VendorName { get; set; }
    public string LogLink { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public string LogName { get; set; }
}

public sealed record CustomerLogRequest : LogRequest
{
    public int RequestLogId { get; set; }
}

public sealed record CustomerOrderLogRequest : LogRequest
{
    public int VendorId { get; set; }
}
public sealed record CustomerPaymentProofRequest : LogRequest
{
    public string VendorCode { get; set; }
}
public record LogRequest
{
    public int PageSize { get; set; } = 10;
    public int PageNumber { get; set; } = 1;
    public string SearchText { get; set; }
}
public record WalletFundRequest
{
    public int PageSize { get; set; } = 10;
    public int PageNumber { get; set; } = 1;
    public string TrendFilter { get; set; }
}

public sealed record CustomerRequestLogRecord
{
    public int Id { get; set; }
    public string CustomerName { get; set; }
    public string LogName { get; set; }
    public string Description { get; set; }
    public string WhatsAppNumber { get; set; }
    public string Email { get; set; }
}

public sealed record TotalRequestCharges
{
    public decimal Daily { get; set; }
    public decimal CurrentMonth { get; set; }
    public string DailyText { get; set; }
    public string CurrentMonthText { get; set; }
}

public sealed record TempEmailRequest
{
    public string UserId { get; set; }
    public string UserName { get; set; }
    public string DomainName { get; set; }
}

public class InboxesMessageResponse
{
    public string uid { get; set; }
    public string from { get; set; }
    public string to { get; set; }
    public string subject { get; set; }
    public DateTime created_at { get; set; }
    public int created_at_epoch { get; set; }
}

public class InboxesSingleMessageResponse
{
    public string uid { get; set; }
    public string from { get; set; }
    public string to { get; set; }
    public string subject { get; set; }
    public DateTime created_at { get; set; }
    public int created_at_epoch { get; set; }
    public string text { get; set; }
    public string html { get; set; }
}

public class CustomerMessageResponse
{
    public string From { get; set; }
    public string To { get; set; }
    public string Subject { get; set; }
    public string Message { get; set; }
    public DateTime CreatedAt { get; set; }
}

public record CustomerBankResponse
{
    public string AccountName { get; set; }
    public string AccountNumber { get; set; }
    public string BankName { get; set; }
}

public record CustomerAccountResponse : CustomerBankResponse
{
    public string Status { get; set; }
    public string DateCreated { get; set; }
}

public record CustomerBankRequest
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string BVN { get; set; }
    public string PhoneNumber { get; set; }
    public string BankCode { get; set; }
}
public record AddNewVirtualParam
{
    //CustomerVirtualAccount
    public int UserId { get; set; }

    public string AcctountName { get; set; }

    public string AcctountNumber { get; set; }

    public string BankCode { get; set; }

    public string BankName { get; set; }
}

public record InitiatePaymentRequest
{
    public int UserId { get; set; }
    public decimal Amount { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string PhoneNumber { get; set; }
}

// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
public class CustomerDetails
{
    public string customer_name { get; set; }
    public string customer_email { get; set; }
    public string customer_phone { get; set; }
}

public class Data
{
    public string trans_status { get; set; }
    public string message { get; set; }
    public string transmode { get; set; }
    public string feeby { get; set; }
    public string reference { get; set; }
    public TransDetails TransDetails { get; set; }
    public CustomerDetails CustomerDetails { get; set; }
    public int transerror { get; set; }
    public string paid_through { get; set; }
    public string isflagged { get; set; }
}

public class WebhookResponse
{
    public string event_type { get; set; }
    public Data data { get; set; }
}

public class TransDetails
{
    public string transref { get; set; }
    public string clientref { get; set; }
    public string amountpaid { get; set; }
    public string amount_settled { get; set; }
    public string fee { get; set; }
    public string currency { get; set; }
    public string transtoken { get; set; }
    public string previous_bal { get; set; }
    public string new_bal { get; set; }
    public string payment_channel { get; set; }
    public string payment_time { get; set; }
    public string redirect_url { get; set; }
    public string transmsg { get; set; }
}

public record StaticWalletRequest
{
    public int UserId { get; set; }
    public string Currency { get; set; }
}

public record UserStaticWalletResponse
{
    public string Address { get; set; }
    public string Name { get; set; }
    public string Code { get; set; }
    public string IconPath { get; set; }
}

public record WalletAddressResponse
{
    public string Address { get; set; }
    public string Name { get; set; }
    public string Code { get; set; }
}

public record DaradCustomerTransactionRecords
{
    public string DebitAmountText { get; set; }
    public decimal? DebitAmount { get; set; }
    public string CreditAmountText { get; set; }
    public decimal? CreditAmount { get; set; }
    public string WalletBalanceText { get; set; }
    public decimal WalletBalance { get; set; }
    public DateTime? TransactionDate { get; set; }
}
public record FundWalletTransactionRecords
{
    public string CreditAmountText { get; set; }
    public decimal? CreditAmount { get; set; }
    public DateTime? TransactionDate { get; set; }
    public string PaymentMode { get; set; }
}
public record WalletTransactionRecords
{
    public string ProductName { get; set; }
    public decimal DebitAmount { get; set; }
    public decimal WalletBalance { get; set; }
    public DateTime? TransactionDate { get; set; }
    public string ProductImage { get; set; }
}
public record TransactionsResponse
{
    public DateTime? TransactionDate { get; set; }
    public IEnumerable<WalletTransactionRecords> TransactionRecords { get; set; }
}
public class ICustomerDetails
{
    public string AccountName { get; set; }
    public string AccountNo { get; set; }
    public string BankName { get; set; }
    public string TrackingId { get; set; }
}

public class RootResponse
{
    public string event_type { get; set; }
    public string event_status { get; set; }
    public string AccountNo { get; set; }
    public string paid_through { get; set; }
    public string trans_status { get; set; }
    public string transmode { get; set; }
    public string Reference { get; set; }
    public string SourceName { get; set; }
    public string AmountPaid { get; set; }
    public double SettledAmount { get; set; }
    public double Charged { get; set; }
    public string TrackingID { get; set; }
    public string TrackingRef { get; set; }
    public string AccountRef { get; set; }
    public string ClientID { get; set; }
    public string transactionType { get; set; }
    public string Narration { get; set; }
    public ICustomerDetails CustomerDetails { get; set; }
    public SourceDetails SourceDetails { get; set; }
    public string transerror { get; set; }
}

public class SourceDetails
{
    public string SourceName { get; set; }
    public string SourceAcct { get; set; }
    public string SourceBank { get; set; }
    public string Narration { get; set; }
}