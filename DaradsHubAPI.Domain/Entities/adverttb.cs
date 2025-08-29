using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DaradsHubAPI.Domain.Entities;

[Table("adverttb")]
public partial class adverttb
{
    public int? id { get; set; }

    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int user_id { get; set; }

    public int? promostatus { get; set; }

    public int? smscan_unit { get; set; }

    public int? emailcan_unit { get; set; }

    [Column(TypeName = "date")]
    public DateTime? start_date { get; set; }

    [Column(TypeName = "date")]
    public DateTime? end_date { get; set; }

    public int? promo_days { get; set; }

    [Column(TypeName = "date")]
    public DateTime? regdate { get; set; }
}
