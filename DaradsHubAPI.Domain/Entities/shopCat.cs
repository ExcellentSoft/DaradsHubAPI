using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DaradsHubAPI.Domain.Entities;

[Table("shopCat")]
public partial class shopCat
{
    public int id { get; set; }

    [StringLength(50)]
    public string? catname { get; set; }

    [StringLength(2)]
    public string? status { get; set; }
    public int userId { get; set; }
    public long ProductId { get; set; }
    public int Quantity { get; set; }
    public bool SendNotification { get; set; }
    public string? sessionId { get; set; }

}