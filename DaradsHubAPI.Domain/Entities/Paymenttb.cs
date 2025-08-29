using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DaradsHubAPI.Domain.Entities;
#nullable disable

[Table("Paymenttb")]
public partial class Paymenttb
{
    [Key]
    public int id { get; set; }

    [StringLength(50)]
    public string fullname { get; set; }

    [StringLength(50)]
    public string username { get; set; }

    [StringLength(50)]
    public string userID { get; set; }

    public int? subID { get; set; }

    [StringLength(50)]
    public string bankname { get; set; }

    [StringLength(50)]
    public string refnumber { get; set; }

    [StringLength(50)]
    public string paymentTitle { get; set; }

    [StringLength(50)]
    public string amount { get; set; }

    public DateTime? paydate { get; set; }
}

public partial class PaymentLog
{
    [Key]
    public int id { get; set; }



    [StringLength(50)]
    public string Username { get; set; }

    [StringLength(50)]
    public string userID { get; set; }


    [StringLength(50)]
    public string bankname { get; set; }

    [StringLength(50)]
    public string Refnumber { get; set; }
    //
    [StringLength(50)]
    public string Walletcode { get; set; }
    [StringLength(50)]
    public string PaymentTitle { get; set; }

    [StringLength(50)]
    public string Amount { get; set; }

    public DateTime? Paydate { get; set; }
    public int Status { get; set; }//0, initiated, 1=processed; 2=confirmed and credited
    public DateTime? Updatedate { get; set; }
    public int? TransId { get; set; }
}
