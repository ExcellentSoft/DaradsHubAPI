using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DaradsHubAPI.Domain.Entities;
#nullable disable

[Table("servy")]
public partial class servy
{
    public int id { get; set; }

    public int? user_id { get; set; }

    public string title { get; set; }

    public string slug { get; set; }

    [StringLength(6)]
    public string price { get; set; }

    public int? state_id { get; set; }

    [StringLength(150)]
    public string location { get; set; }

    [StringLength(50)]
    public string duration { get; set; }

    public string description { get; set; }

    public string image { get; set; }

    public int? status { get; set; }

    [StringLength(10)]
    public string type { get; set; }

    public DateTime? created_at { get; set; }

    public DateTime? updated_at { get; set; }

    public string pubId { get; set; }

    [StringLength(10)]
    public string post_type { get; set; }

    public int? promostatus { get; set; }

    [StringLength(50)]
    public string promotionCode { get; set; }

    [StringLength(50)]
    public string tags { get; set; }

    [StringLength(50)]
    public string servicePostCode { get; set; }

    public int? discount { get; set; }

    public int? topcatId { get; set; }

    [StringLength(50)]
    public string stockstatus { get; set; }

    public DateTime? promoEddate { get; set; }

    public int? promoDays { get; set; }

    public int? category_id { get; set; }

    public DateTime? promoStdate { get; set; }

    [StringLength(500)]
    public string socialLink { get; set; }

    [StringLength(1000)]
    public string VideoPath { get; set; }
}
