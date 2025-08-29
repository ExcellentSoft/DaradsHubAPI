using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DaradsHubAPI.Domain.Entities;
#nullable disable
[Table("searchlog")]
public partial class searchlog
{
    public int id { get; set; }

    [StringLength(50)]
    public string keywords { get; set; }

    public DateTime? dtime { get; set; }

    public int? catID { get; set; }

    [StringLength(50)]
    public string IPadds { get; set; }

    public string customerId { get; set; }

    public int? status { get; set; }
}
