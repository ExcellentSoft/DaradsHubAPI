using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DaradsHubAPI.Domain.Entities;
#nullable disable
[Table("locationtb")]
public partial class locationtb
{
    public int id { get; set; }

    public int? stateid { get; set; }

    [StringLength(50)]
    public string state { get; set; }

    [StringLength(50)]
    public string city { get; set; }

    [StringLength(50)]
    public string lga { get; set; }

    [StringLength(100)]
    public string addres1 { get; set; }

    [StringLength(100)]
    public string addres2 { get; set; }

    [StringLength(256)]
    public string lat { get; set; }

    [StringLength(256)]
    public string lng { get; set; }

    [StringLength(50)]
    public string country { get; set; }

    public int? user_id { get; set; }

    [StringLength(130)]
    public string customer_id { get; set; }

    public string agent_id { get; set; }
}
