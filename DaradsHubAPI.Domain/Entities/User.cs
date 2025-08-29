using Microsoft.AspNetCore.Identity;
using static DaradsHubAPI.Domain.Enums.Enum;

namespace DaradsHubAPI.Domain.Entities;
#nullable disable
public class User : IdentityUser
{

    public int? Is_customer { get; set; } = 0;
    public int? Is_admin { get; set; } = 0;
    public int? Is_Vendor { get; set; } = 0;
    public int Is_VendorCustomer { get; set; }
    public int? Is_agent { get; set; } = 0;
    public int Is_ApiClient_Customer { get; set; } = 0;
    public int? Is_ApiClient { get; set; } = 0;
    public string Customer_Api_ClientId { get; set; }
    public string customerName { get; set; }
    //UserType=:R, D
    public string SignUpType { get; set; } = string.Empty;
    public int Gwallet { get; set; }
    public EntityStatusEnum Status { get; set; }
}

public sealed record LoginResponse
{
    public string Name { get; set; }
    public string UserId { get; set; }
    public string Email { get; set; }
    public string Photo { get; set; }
    public string Token { get; set; }
    public long Expires { get; set; }
    public VendorDetails VendorDetails { get; set; }
    public DateTimeOffset ExpiresTime { get; set; }
}

public sealed record CustomerLoginResponse
{
    public string Name { get; set; }
    public string UserId { get; set; }
    public int UserIdInt { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public bool IsVerify { get; set; }
    public bool Is2FA { get; set; }
    public string Photo { get; set; }
    public string Token { get; set; }
    public long Expires { get; set; }
    public DateTimeOffset ExpiresTime { get; set; }
    public int? IsCustomer { get; set; }
    public int? IsAgent { get; set; }
    public int? IsAdmin { get; set; }
}

public sealed record VendorDetails
{
    public string Name { get; set; }
    public string PhoneNumber { get; set; }
    public string BankName { get; set; }
    public string AccountName { get; set; }
    public string AccountNumber { get; set; }
    public bool IsVerify { get; set; }
    public string VendorPageName { get; set; }
    public string CustomerCareWhatsAppNumber { get; set; }
}

public record LoginRequest : ForgetPasswordRequest
{
    public string Password { get; set; } = default!;
}

public record ForgetPasswordRequest
{
    public string Email { get; set; } = default!;
}

public class ResetPasswordRequest
{
    public string Code { get; set; } = default!;
    public string NewPassword { get; set; } = default!;
    public string ConfirmPassword { get; set; } = default!;
}
