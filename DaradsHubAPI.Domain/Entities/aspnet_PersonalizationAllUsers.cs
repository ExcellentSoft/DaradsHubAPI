using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DaradsHubAPI.Domain.Entities;
#nullable disable
public partial class aspnet_PersonalizationAllUsers
{
    [Key]
    public Guid PathId { get; set; }

    [Column(TypeName = "image")]
    [Required]
    public byte[] PageSettings { get; set; }

    public DateTime LastUpdatedDate { get; set; }

    public virtual aspnet_Paths aspnet_Paths { get; set; }
}
