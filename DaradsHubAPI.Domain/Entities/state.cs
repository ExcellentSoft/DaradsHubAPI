using System.ComponentModel.DataAnnotations;

namespace DaradsHubAPI.Domain.Entities;
#nullable disable
public partial class state
{
    public int id { get; set; }

    [Required]
    [StringLength(255)]
    public string name { get; set; }
}
