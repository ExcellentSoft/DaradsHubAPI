using System.ComponentModel.DataAnnotations;

namespace DaradsHubAPI.Domain.Entities;
#nullable disable
public class referralCommission
{
    [Key]
    public int Id { get; set; }
    public int FromUserId { get; set; }
    public int RefByUserId { get; set; }
    public string RefByEmail { get; set; }
    public decimal ChargeAmt { get; set; }
    public DateTime CreatedDate { get; set; }
    public string PurchaseItem { get; set; }
}
public class UserreferralBalance
{
    public int UserId { get; set; }
    [Key]
    public int Id { get; set; }
    public decimal Balance { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime ModifiedDate { get; set; }
    public string LastOperation { get; set; } //Credit/Debit
}
public class WithdrawCommissionHistory
{
    public int UserId { get; set; }
    [Key]
    public int Id { get; set; }
    public decimal WithdrawAmt { get; set; }
    public decimal Balance { get; set; }
    public DateTime CreatedDate { get; set; }
}
