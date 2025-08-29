using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DaradsHubAPI.Domain.Entities;
#nullable disable

[Table("subscriptionplan")]
public partial class subscriptionplan
{
    public int id { get; set; }

    [StringLength(50)]
    public string username { get; set; }

    public int? userid { get; set; }

    public int subStatus { get; set; }

    [StringLength(10)]
    public string Amount { get; set; }

    public int? duration { get; set; }

    public DateTime? start_date { get; set; }

    public DateTime? end_date { get; set; }

    [StringLength(50)]
    public string plan_type { get; set; }

    public string subplan_Type { get; set; }

    public int? SMS_unit { get; set; }

    public int? Email_unit { get; set; }

    public int? Call_unit { get; set; }

    public DateTime? regdate { get; set; }

    public int? postnum { get; set; }

    [StringLength(50)]
    public string FirstTime { get; set; }

    public DateTime? up_dated { get; set; }

    [StringLength(10)]
    public string paymentstatus { get; set; }
}
