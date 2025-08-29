using System.ComponentModel.DataAnnotations;

namespace DaradsHubAPI.Domain.Entities;
#nullable disable

public class Vendorpaymenthistory
{
    [Key]
    public int Id { get; set; }
    public string UserId { get; set; }
    public string AccountNumber { get; set; }
    public string BankName { get; set; }
    public decimal Amt { get; set; }
    public string PayNarration { get; set; }
    public DateTime PayDate { get; set; }

}

public class VpayApiLogin
{
    [Key]
    public int Id { get; set; }
    public string tokens { get; set; }
    public DateTime CreatedDate { get; set; }
}