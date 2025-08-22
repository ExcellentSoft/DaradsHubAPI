using System.ComponentModel.DataAnnotations;

namespace DaradsHubAPI.Domain.Entities;
#nullable disable
public partial class AspNetUser
{
    public AspNetUser()
    {
        AspNetUserClaims = new HashSet<AspNetUserClaim>();
        AspNetUserLogins = new HashSet<AspNetUserLogin>();
        AspNetRoles = new HashSet<AspNetRole>();
    }

    public string Id { get; set; }

    [StringLength(256)]
    public string Email { get; set; }

    public bool EmailConfirmed { get; set; }

    public string PasswordHash { get; set; }

    public string SecurityStamp { get; set; }

    public string PhoneNumber { get; set; }

    public bool PhoneNumberConfirmed { get; set; }

    public bool TwoFactorEnabled { get; set; }

    public DateTime? LockoutEndDateUtc { get; set; }

    public bool LockoutEnabled { get; set; }

    public int AccessFailedCount { get; set; }

    [Required]
    [StringLength(256)]
    public string UserName { get; set; }

    public int? Is_agent { get; set; }

    public int? Is_customer { get; set; }

    public int? Is_admin { get; set; }

    [StringLength(50)]
    public string customerName { get; set; }

    [StringLength(16)]
    public string ad_accountpass { get; set; }

    public int gwallet { get; set; }

    public virtual ICollection<AspNetUserClaim> AspNetUserClaims { get; set; }

    public virtual ICollection<AspNetUserLogin> AspNetUserLogins { get; set; }

    public virtual ICollection<AspNetRole> AspNetRoles { get; set; }
}
