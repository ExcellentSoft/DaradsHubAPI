using System.ComponentModel.DataAnnotations;

namespace DaradsHubAPI.Domain.Entities;
#nullable disable
public class SetvendorLog
{
    [Key]
    public int Id { get; set; }
    public int LogTypeId { get; set; }
    public string VendorId { get; set; }
    public decimal salesPrice { get; set; }
    public decimal CostPrice { get; set; }
    public DateTime CreateDate { get; set; }
}
