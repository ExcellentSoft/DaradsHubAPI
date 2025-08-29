using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DaradsHubAPI.Domain.Entities;
#nullable disable
[Table("Withdrawtb")]
public partial class Withdrawtb
{
    public int id { get; set; }

    public int? userId { get; set; }

    public decimal? WAmount { get; set; }

    [StringLength(50)]
    public string accountname { get; set; }

    [StringLength(50)]
    public string bankname { get; set; }

    [StringLength(50)]
    public string accountnumber { get; set; }

    [StringLength(50)]
    public string PayStatus { get; set; }

    public DateTime? requestDate { get; set; }

    public DateTime? PayDate { get; set; }
}
