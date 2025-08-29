using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DaradsHubAPI.Domain.Entities;
#nullable disable
public partial class aspnet_Applications
{
    public aspnet_Applications()
    {
        aspnet_Membership = new HashSet<aspnet_Membership>();
        aspnet_Paths = new HashSet<aspnet_Paths>();
        aspnet_Roles = new HashSet<aspnet_Roles>();
        aspnet_Users = new HashSet<aspnet_Users>();
    }

    [Required]
    [StringLength(256)]
    public string ApplicationName { get; set; }

    [Required]
    [StringLength(256)]
    public string LoweredApplicationName { get; set; }

    [Key]
    public Guid ApplicationId { get; set; }

    [StringLength(256)]
    public string Description { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
    public virtual ICollection<aspnet_Membership> aspnet_Membership { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
    public virtual ICollection<aspnet_Paths> aspnet_Paths { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
    public virtual ICollection<aspnet_Roles> aspnet_Roles { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
    public virtual ICollection<aspnet_Users> aspnet_Users { get; set; }
}
