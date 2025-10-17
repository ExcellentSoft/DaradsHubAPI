using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DaradsHubAPI.Domain.Entities;
#nullable disable
[Table("agenttb")]
public partial class agenttb
{
    public string userid { get; set; }

    [Key]
    public int id { get; set; }

    public int? stateid { get; set; }

    public int? locid { get; set; }

    [StringLength(50)]
    public string username { get; set; }

    [StringLength(50)]
    public string fullname { get; set; }

    [StringLength(50)]
    public string phonenum { get; set; }

    [StringLength(50)]
    public string email { get; set; }

    [Column(TypeName = "date")]
    public DateTime? regdate { get; set; }

    [StringLength(20)]
    public string promotionCode { get; set; }

    [StringLength(20)]
    public string ConfirmCode { get; set; }

    public string IDimagePath { get; set; }

    [StringLength(50)]
    public string acctype { get; set; }

    public int? Status { get; set; }

    [Column(TypeName = "text")]
    public string aboutme { get; set; }

    public string profileimg { get; set; }

    [StringLength(6)]
    public string PIN { get; set; }
}

public class CashPayment
{
    /*
     cmd.Parameters.AddWithValue("@WalletUserId", txtUserWalletID.Text);
                    cmd.Parameters.AddWithValue("@PhoneNumber", txtPhoneNumber.Text);
                    cmd.Parameters.AddWithValue("@Amount", PaidAmount);
                    cmd.Parameters.AddWithValue("@BankName", txtbankName.Text);
                    cmd.Parameters.AddWithValue("@DepositorName", txtDname.Text);
                    cmd.Parameters.AddWithValue("@PaidFromAccountName", txtPayfromAccount.Text);
                    cmd.Parameters.AddWithValue("@PayDate", dt);
                    cmd.Parameters.AddWithValue("@Status", "Submitted");* 
     */
    [Key]
    public int Id { get; set; }
    public string WalletUserId { get; set; }
    public string PhoneNumber { get; set; }
    public decimal Amount { get; set; }
    public string BankName { get; set; }
    public string DepositorName { get; set; }
    public string PaidFromAccountName { get; set; }
    public DateTime PayDate { get; set; }
    public string? Status { get; set; }
    public DateTime? UpdateDate { get; set; }

}