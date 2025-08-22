using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DaradsHubAPI.Domain.Entities;
#nullable disable
public partial class AspNetRole
{
    public AspNetRole()
    {
        AspNetUsers = new HashSet<AspNetUser>();
    }

    public string Id { get; set; }

    [Required]
    [StringLength(256)]
    public string Name { get; set; }

    public virtual ICollection<AspNetUser> AspNetUsers { get; set; }
}
