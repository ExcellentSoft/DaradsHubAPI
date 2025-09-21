using System;
using System.ComponentModel.DataAnnotations;

namespace DaradsHubAPI.Domain.Entities;
#nullable disable
public class userstb
{
    public string userid { get; set; } //-id from authtable
    [Key]
    public int id { get; set; }
    public string username { get; set; }


    public string fullname { get; set; }


    public string phone { get; set; }

    [StringLength(100)]
    public string email { get; set; }

    public DateTime? regdate { get; set; }
    public string NewPassword { get; set; }
    public string mgr_code { get; set; }

    public int? status { get; set; }
    public int? RefByID { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime? ModifiedDate { get; set; }
    //new columns
    public Guid? VerificationId { get; set; }
    public string VerificationCode { get; set; }
    public string myreflink { get; set; }
    public string CountryCode { get; set; } = string.Empty;
    public DateTime? VerificationCreatedDate { get; set; }
    public bool AccountVerificationStatus { get; set; }
    public string MyRefCode { get; set; }
    public string pstatus { get; set; }
    public string WhatsAppNumber { get; set; }
    public string VendorNickName { get; set; }
    public string VirtualAccountDetails { get; set; }
    public string Photo { get; set; }
    public string VpayAccountName { get; set; }
    public string VpayAccountId { get; set; }
    public string VpayAccountNumber { get; set; }
    public string VpayBankName { get; set; }
    public string BusinessName { get; set; }
    public string BusinessEmail { get; set; }
    public bool? IsVPayCreated { get; set; }
    public DateTime? VpayCreatedDate { get; set; }
    public bool? IsAgent { get; set; }
    public bool? IsOnline { get; set; }
    public bool? IsPublicAgent { get; set; }
}

public class UserBankDetails
{
    [Key]
    public int Id { get; set; }
    [StringLength(50)]
    public string AcctName { get; set; }
    [StringLength(50)]
    public string AcctNumber { get; set; }
    [StringLength(50)]
    public string BankName { get; set; }
    [StringLength(50)]
    public string UserId { get; set; }
    public DateTime? CrtDate { get; set; }
    public DateTime? UpdDate { get; set; }
}

public class CryptoCloudUser
{
    [Key]
    public int Id { get; set; }
    [MaxLength(100)]
    public string UserId { get; set; }
    public int WalletId { get; set; }
    [MaxLength(100)]
    public string Address { get; set; }
    [MaxLength(100)]
    public string UUID { get; set; }
    [MaxLength(75)]
    public string CurrencyName { get; set; }
    [MaxLength(20)]
    public string CurrencyCode { get; set; }
    public DateTime TimeCreated { get; set; }
}
