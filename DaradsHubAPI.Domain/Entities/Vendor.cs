using System.ComponentModel.DataAnnotations;
using static DaradsHubAPI.Domain.Enums.Enum;

namespace DaradsHubAPI.Domain.Entities;
#nullable disable
public class VendorWebsite
{
    [Key]
    public int Id { get; set; }
    public int UserId { get; set; }
    [MaxLength(50)]
    public string DomainName { get; set; }
    [MaxLength(200)]
    public string WebsiteLink { get; set; }
    [MaxLength(20)]
    public string Code { get; set; }
    public EntityStatusEnum Status { get; set; }
    public DateTime TimeCreated { get; set; }
    public DateTime TimeUpdated { get; set; }
    [MaxLength(50)]
    public string PageName { get; set; }
    public string SignUpLink { get; set; }
    public string WebsiteLogoPath { get; set; }
    public string WebsiteTitle { get; set; }
}

public class VendorPaymentGateway
{
    [Key]
    public int Id { get; set; }
    public int UserId { get; set; }
    [MaxLength(200)]
    public string GatewayName { get; set; }
    [MaxLength(200)]
    public string ApiKey { get; set; }
    [MaxLength(200)]
    public string ApiSecret { get; set; }
    public EntityStatusEnum Status { get; set; }
    public DateTime TimeCreated { get; set; }
    public DateTime TimeUpdated { get; set; }
}

public class VendorCustomerOrder
{
    [Key]
    public int Id { get; set; }
    public int VendorId { get; set; }
    public int CustomerId { get; set; }
    [MaxLength(250)]
    public string LogType { get; set; }
    [MaxLength(500)]
    public string LogValue { get; set; }
    public decimal Amount { get; set; }
    public decimal ChargeAmount { get; set; }
    public TransactionStatusEnum Status { get; set; }
    public DateTime TimeCreated { get; set; }
    public DateTime TimeUpdated { get; set; }
}

public class VendorCustomerTransaction
{
    [Key]
    public int Id { get; set; }
    public int VendorId { get; set; }
    public int CustomerId { get; set; }
    [MaxLength(50)]
    public TransactionTypeEnum Type { get; set; }
    [MaxLength(30)]
    public string RefNo { get; set; }
    public decimal DR { get; set; }
    public decimal CR { get; set; }
    public TransactionStatusEnum Status { get; set; }
    public decimal WalletBalance { get; set; }
    public DateTime TimeCreated { get; set; }
    public DateTime TimeUpdated { get; set; }
}

public class VendorCustomer
{
    [Key]
    public int Id { get; set; }
    [MaxLength(100)]
    public string UserId { get; set; }
    public int VendorId { get; set; }
    [MaxLength(100)]
    public string Name { get; set; }
    [MaxLength(75)]
    public string Email { get; set; }
    [MaxLength(20)]
    public string PhoneNumber { get; set; }
    [MaxLength(50)]
    public string VirtualAccountNumber { get; set; }
    [MaxLength(200)]
    public string Photo { get; set; }
    public EntityStatusEnum Status { get; set; }
    public DateTime TimeCreated { get; set; }
    public DateTime TimeUpdated { get; set; }
}

public class SiteWallet
{
    [Key]
    public int Id { get; set; }
    [StringLength(75)]
    public string VendorEmail { get; set; }
    [StringLength(50)]
    public string WalletCode { get; set; }
    public decimal LastFundAmt { get; set; }
    public decimal Balance { get; set; }
    public DateTime TimeCreated { get; set; }
    public DateTime TimeUpdated { get; set; }
}

public class SiteTransaction
{
    [Key]
    public int Id { get; set; }
    public int VendorId { get; set; }
    [MaxLength(50)]
    public TransactionTypeEnum Type { get; set; }
    [MaxLength(30)]
    public string RefNo { get; set; }
    public decimal DR { get; set; }
    public decimal CR { get; set; }
    public TransactionStatusEnum Status { get; set; }
    public decimal WalletBalance { get; set; }
    public DateTime TimeCreated { get; set; }
    public DateTime TimeUpdated { get; set; }
}


public class VendorCustomerWallet
{
    [Key]
    public int Id { get; set; }
    [StringLength(75)]
    public string CustomerEmail { get; set; }
    [StringLength(50)]
    public string WalletCode { get; set; }
    public decimal? LastFundAmt { get; set; }
    public decimal? Balance { get; set; }
    public DateTime TimeCreated { get; set; }
    public DateTime TimeUpdated { get; set; }
}

public class VendorCustomerPaymentProof
{
    [Key]
    public int Id { get; set; }
    [MaxLength(20)]
    public string VendorCode { get; set; }
    // public int VendorId { get; set; }
    [MaxLength(75)]
    public string CustomerEmail { get; set; }
    [MaxLength(20)]
    public string PhoneNumber { get; set; }
    [MaxLength(75)]
    public string DepositorName { get; set; }//2
    public decimal Amount { get; set; }//3
    [MaxLength(20)]
    public string BeneficiaryAccountNumber { get; set; }
    [MaxLength(60)]
    public string BeneficiaryAccountName { get; set; }
    [MaxLength(50)]
    public string BankName { get; set; } //4
    [MaxLength(200)]
    public string Photo { get; set; }
    public TransactionStatusEnum Status { get; set; } //5.Status
    public DateTime TimeCreated { get; set; }//6.TimeCreated
    public DateTime TimeUpdated { get; set; }
}

public class VendorCustomerFeedback
{
    [Key]
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public int VendorId { get; set; }
    [MaxLength(200)]
    public string Title { get; set; }
    [MaxLength(900)]
    public string Description { get; set; }
    [MaxLength(900)]
    public string Comment { get; set; }
    public DateTime TimeCreated { get; set; }
    public DateTime TimeUpdated { get; set; }
}