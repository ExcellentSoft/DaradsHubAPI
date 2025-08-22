using System;
using System.ComponentModel.DataAnnotations;

namespace DaradsHubAPI.Domain.Entities;

#nullable disable
public class AuditTrail

{
    [Key]
    public int Id { get; set; }
    public string UserId { get; set; }

    public string ActivityType { get; set; }
    public string ActivityDesc { get; set; }
    public DateTime LastLoginDate { get; set; } = Convert.ToDateTime("1/ 1/1753 12:00:00 AM");
    public DateTime LastLogOutDate { get; set; } = Convert.ToDateTime("1/ 1/1753 12:00:00 AM");
    public DateTime? CreatedDate { get; set; }
}
