using System;
using System.ComponentModel.DataAnnotations;

namespace DaradsHubAPI.Domain.Entities;

#nullable disable
public class Influencertb
{
    public string name { get; set; }

    [Key]
    public int id { get; set; }

    public string email { get; set; }

    public string phonenumber { get; set; }
    public string InviteCode { get; set; }
    public string refLink1 { get; set; }
    public decimal? bal { get; set; }
    public DateTime updateDate { get; set; }
}
