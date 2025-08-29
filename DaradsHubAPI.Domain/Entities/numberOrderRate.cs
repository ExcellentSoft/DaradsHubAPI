using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaradsHubAPI.Domain.Entities
{
    public class numberOrderRate
    {
        [Key]
        public int Id { get; set; }
        public decimal DollarRatevalue { get; set; }
        public decimal TopUpvalue { get; set; }
        public decimal Discount { get; set; }
    }
}
