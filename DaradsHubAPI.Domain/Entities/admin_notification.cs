using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DaradsHubAPI.Domain.Entities;

#nullable disable
public partial class admin_notification
{
    public int id { get; set; }

    [StringLength(30)]
    public string title { get; set; }

    [Column(TypeName = "text")]
    public string Message { get; set; }

    [StringLength(14)]
    public string c_phonenum { get; set; }

    [StringLength(150)]
    public string c_email { get; set; }

    public string UserID { get; set; }

    [Column(TypeName = "date")]
    public DateTime? ndate { get; set; }

    public TimeSpan? ntime { get; set; }

    [StringLength(50)]
    public string pagename { get; set; }
}
