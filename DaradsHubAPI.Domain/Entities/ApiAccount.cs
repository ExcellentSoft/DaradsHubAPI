using System;
using System.ComponentModel.DataAnnotations;

namespace DaradsHubAPI.Domain.Entities;
#nullable disable
public class ApiAccount

{
    [Key]
    public long Id { get; set; }
    public string ApiKey { get; set; }
    public string AuthCode { get; set; }
    public decimal WalletBal { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public string PassWord { get; set; }
    public DateTime? CreatedDate { get; set; }
    public bool IsDeleted { get; set; }
    public bool IsAccountValid { get; set; }
}
