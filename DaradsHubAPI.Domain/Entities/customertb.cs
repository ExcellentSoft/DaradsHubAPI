using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DaradsHubAPI.Domain.Entities;

#nullable disable
[Table("customertb")]
public partial class customertb
{
    public int id { get; set; }

    [StringLength(128)]
    public string userid { get; set; }

    [StringLength(50)]
    public string email { get; set; }

    [StringLength(50)]
    public string phonenum { get; set; }

    public int? locid { get; set; }

    public int? sub_status { get; set; }

    [Column(TypeName = "date")]
    public DateTime? regdate { get; set; }

    public int? agreet { get; set; }

    [StringLength(128)]
    public string order_trackCode { get; set; }

    public bool? isPhoneConfirm { get; set; }
}

public class CustomerTempEmail
{
    [Key]
    public int Id { get; set; }
    public int UserId { get; set; }
    [MaxLength(80)]
    public string Email { get; set; }
    public bool IsActive { get; set; }
    public DateTime TimeCreated { get; set; }
    public DateTime TimeUpdated { get; set; }
}

public class CustomerMessage
{
    [Key]
    public int Id { get; set; }
    public int UserId { get; set; }
    [MaxLength(100)]
    public string From { get; set; }
    [MaxLength(100)]
    public string To { get; set; }
    [MaxLength(200)]
    public string Subject { get; set; }
    public string Message { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CustomerVirtualAccount
{
    [Key]
    public int Id { get; set; }
    public int UserId { get; set; }
    [MaxLength(100)]
    public string TrackingRef { get; set; }
    [MaxLength(100)]
    public string TrackingId { get; set; }
    [MaxLength(200)]
    public string AcctountName { get; set; }
    [MaxLength(20)]
    public string AcctountNumber { get; set; }
    [MaxLength(10)]
    public string BankCode { get; set; }
    [MaxLength(70)]
    public string BankName { get; set; }
    public DateTime TimeCreated { get; set; }
    public int Status { get; set; }
}
