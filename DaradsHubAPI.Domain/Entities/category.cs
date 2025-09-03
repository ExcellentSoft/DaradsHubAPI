using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaradsHubAPI.Domain.Entities;
public partial class category
{
    public int id { get; set; }
    [StringLength(50)]
    public string name { get; set; } = default!;
    [StringLength(200)]
    public string? icon { get; set; }
    public int? status { get; set; }
    public int vendorId { get; set; }
    public string? Description { get; set; }
}

public partial class SubCategory
{
    [Key]
    public long Id { get; set; }
    [StringLength(250)]
    public string Name { get; set; } = default!;
    public int CategoryId { get; set; }

}