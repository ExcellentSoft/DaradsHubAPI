using System.ComponentModel.DataAnnotations;
using static DaradsHubAPI.Domain.Enums.Enum;

namespace DaradsHubAPI.Domain.Entities;
#nullable disable
public class AllLogs
{
    [Key]
    public int Id { get; set; }
    public string LogValues { get; set; }
    public int TypeId { get; set; }
    public string UploadBy { get; set; }
    public string status { get; set; }
    public string purchasedBy { get; set; }
    public DateTime? purchaseDate { get; set; }
    public string IsDeleted { get; set; }
    public string PurchaseUser { get; set; }
    public string LogVendorId { get; set; }
    public decimal CostPrice { get; set; }
}
public class GetAllLogs
{
    public string LogValues { get; set; }

    public int TypeId { get; set; }

    public string status { get; set; }

    public string PurchasedBy { get; set; }
    public string PurchaseUser { get; set; }
    public int Id { get; set; }
    public DateTime? purchaseDate { get; set; }
    public int IsDeleted { get; set; }
}

public class OtpVerificationLog
{
    [Key]
    public int Id { get; set; }
    [MaxLength(75)]
    public string Recipient { get; set; } = default!;
    [MaxLength(100)]
    public string UserId { get; set; }
    [MaxLength(10)]
    public string Code { get; set; } = default!;
    public OtpCodeStatusEnum Status { get; set; }
    public OtpVerificationPurposeEnum Purpose { get; set; }
    public DateTimeOffset? ConfirmedOn { get; set; }
    public DateTime TimeCreated { get; set; }
    public DateTime TimeUpdated { get; set; }
}
public sealed record SendOtpRequest
{
    public string UserId { get; set; }
    public string FirstName { get; set; } = default!;
    public string UserEmail { get; set; } = default!;
    public string SenderName { get; set; } = default!;
    public string Code { get; set; } = default!;
    public string SendingMode { get; set; } = default!;
    public OtpVerificationPurposeEnum Purpose { get; set; }
}

public sealed record ValidateOtpRequest
{
    public string UserId { get; set; }
    public string Code { get; set; } = default!;
    public OtpVerificationPurposeEnum Purpose { get; set; }
}
