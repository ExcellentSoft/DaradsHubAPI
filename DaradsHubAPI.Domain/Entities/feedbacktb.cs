using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DaradsHubAPI.Domain.Entities;

#nullable disable
[Table("feedbacktb")]
public partial class feedbacktb
{
    public int id { get; set; }

    [StringLength(50)]
    public string name { get; set; }

    [Column(TypeName = "text")]
    public string comment { get; set; }

    [StringLength(550)]
    public string message1 { get; set; }

    [StringLength(550)]
    public string message2 { get; set; }

    public DateTime? postDate { get; set; }
}
