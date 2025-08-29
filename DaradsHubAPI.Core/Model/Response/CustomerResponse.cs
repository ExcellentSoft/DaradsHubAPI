using DaradsHubAPI.Domain.Entities;
using DaradsHubAPI.Shared.Customs;
using Microsoft.AspNetCore.Http;
using static DaradsHubAPI.Domain.Enums.Enum;

namespace DaradsHubAPI.Core.Model.Response;

public record CreateCustomerRequest
{
    public string FullName { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string PhoneNumber { get; set; } = default!;
    public string Password { get; set; } = default!;
    public string ConfirmPassword { get; set; } = default!;


    public User ToUser()
    {
        return new User
        {
            Email = Email,
            Is_customer = 1,
            NormalizedEmail = Email.ToUpper(),
            UserName = Email,
            NormalizedUserName = Email.ToUpper(),
            Status = EntityStatusEnum.InActive,
            SignUpType = "C",
            PhoneNumber = PhoneNumber
        };
    }

    public userstb ToCustomer(string userId)
    {
        return new userstb
        {
            userid = userId,
            email = Email,
            phone = PhoneNumber,
            status = (int)EntityStatusEnum.InActive,
            fullname = FullName,
            regdate = GetLocalDateTime.CurrentDateTime()
        };
    }

    public wallettb ToCustomerWallet(string code)
    {
        return new wallettb
        {

            UserId = Email,
            Balance = 0.0M,
            CreatedDate = GetLocalDateTime.CurrentDateTime(),
            Walletcode = code,
            NewAcct = "Y",
            UpdateDate = GetLocalDateTime.CurrentDateTime()
        };
    }
}

public record CustomerProfileResponse
{
    public string UserName { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string PhoneNumber { get; set; } = default!;
    public string Photo { get; set; } = default!;
    public string UserId { get; set; } = default!;
    public int UserIdInt { get; set; }
    public decimal WalletBalance { get; set; }
    public IEnumerable<VirtualAccountDetails> VirtualAccountDetails { get; set; } = default!;
}

public sealed record VirtualAccountDetails
{
    public string? BankName { get; set; }
    public string? AccountName { get; set; }
    public string? AccountNumber { get; set; }
}


public record CustomerProfileRequest
{
    public string PhoneNumber { get; set; } = default!;
    public string UserName { get; set; } = default!;
    public IFormFile? Photo { get; set; }
}