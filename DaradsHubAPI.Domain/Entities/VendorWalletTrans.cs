using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaradsHubAPI.Domain.Entities;
#nullable disable

public class VendorWalletTrans
{
    [Key]
    public int Id { get; set; }
    public int WalletId { get; set; }
    public string UserId { get; set; }
    public decimal Balance { get; set; }
    public decimal TransAmt { get; set; }
    public decimal CR { get; set; }
    public decimal DR { get; set; }
    public DateTime CreatedDate { get; set; }
    public string TransNarration { get; set; } //e.g LogType Name or payment type or withdrawal, Fund-Wallet
    public string RefNumber { get; set; }
    public string TransMedium { get; set; } = "N";
}
