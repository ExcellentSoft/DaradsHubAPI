using System.ComponentModel.DataAnnotations;

namespace DaradsHubAPI.Domain.Entities;
#nullable disable
public class LogTypes
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal BulkPrice { get; set; }
    public int Status { get; set; }
    public string Image { get; set; }
}
