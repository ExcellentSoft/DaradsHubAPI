using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DaradsHubAPI.Domain.Entities;
#nullable disable

[Table("requesttb")]
public partial class requesttb
{
    public int id { get; set; }

    [StringLength(50)]
    public string fullname { get; set; }

    public int? cateryID { get; set; }

    [StringLength(250)]
    public string request_ { get; set; }

    [StringLength(20)]
    public string phone { get; set; }

    [StringLength(50)]
    public string email { get; set; }

    public int? locationID { get; set; }

    [StringLength(130)]
    public string userID { get; set; }

    [StringLength(10)]
    public string providerType { get; set; }
}
