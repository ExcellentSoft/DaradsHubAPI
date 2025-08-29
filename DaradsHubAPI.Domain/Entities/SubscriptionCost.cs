using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DaradsHubAPI.Domain.Entities;
#nullable disable

[Table("SubscriptionCost")]
public partial class SubscriptionCost
{
    public int id { get; set; }

    [StringLength(50)]
    public string Subs_plan { get; set; }

    public decimal? Sub_cost { get; set; }

    public int? Sub_duration { get; set; }

    [StringLength(250)]
    public string Sub_title { get; set; }

    public int? SMS_unit { get; set; }

    public int? Email_unit { get; set; }

    public int? Call_unit { get; set; }

    public int? Post_unit { get; set; }
}
