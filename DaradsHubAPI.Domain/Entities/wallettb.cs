using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DaradsHubAPI.Domain.Entities;
#nullable disable

[Table("wallettb")]
public partial class wallettb
{
    [Key]
    public int id { get; set; }

    public string UserId { get; set; }

    [Required]
    [StringLength(50)]
    public string Walletcode { get; set; }

    public decimal? LastfundAmt { get; set; }

    public decimal? Balance { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdateDate { get; set; }
    public string FundBy { get; set; }
    public string NewAcct { get; set; }

}
public partial class VendorWallet
{
    [Key]
    public int Id { get; set; }
    public string UserId { get; set; } //AspNetUser.id or userstb.userid . Not email address

    public string Walletcode { get; set; }

    public decimal? LastfundAmt { get; set; }

    public decimal Balance { get; set; }
    public DateTime CreatedDate { get; set; }

    public DateTime? UpdateDate { get; set; }
    public int SalesCount { get; set; }



}

public partial class GwalletTran
{
    [Key]
    public int id { get; set; }


    public string transType { get; set; }


    public string refNo { get; set; }
    [StringLength(50)]
    public string areaCode { get; set; }


    public string userName { get; set; }

    public decimal amt { get; set; }
    public decimal? DR { get; set; }
    public decimal? CR { get; set; } = 0.0m;
    public string transStatus { get; set; }

    public string Status { get; set; }
    public decimal? walletBal { get; set; }

    public DateTime transdate { get; set; }

    [Column(TypeName = "text")]
    public string orderItem { get; set; }
    public string transMedium { get; set; }
    public int? QTY { get; set; }
    public int? orderid { get; set; }
}

