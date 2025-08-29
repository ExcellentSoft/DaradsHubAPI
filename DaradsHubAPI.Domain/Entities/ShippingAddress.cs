using System.ComponentModel.DataAnnotations;

namespace DaradsHubAPI.Domain.Entities;
#nullable disable
public partial class ShippingAddress
{
    [Key]
    public long Id { get; set; }
    public int CustomerId { get; set; }
    [MaxLength(255)]
    public string Address { get; set; }
    [MaxLength(70)]
    public string City { get; set; }
    [MaxLength(70)]
    public string State { get; set; }
    [MaxLength(70)]
    public string Country { get; set; }
    [MaxLength(75)]
    public string Email { get; set; }
    [MaxLength(20)]
    public string PhoneNumber { get; set; }
}
