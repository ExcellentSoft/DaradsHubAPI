using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaradsHubAPI.Domain.Entities
{
    public class Review
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public long ProductId { get; set; }

        [Required]
        [StringLength(1000)]
        public string Content { get; set; } = default!;

        [Required]
        [Range(1, 5)]
        public int Rating { get; set; }

        public DateTime ReviewDate { get; set; } = DateTime.Now;

        // public Product? Product { get; set; }
    }

    public class HubReview
    {
        [Key]
        public long Id { get; set; }
        [Required]
        public long ProductId { get; set; }
        public int ReviewById { get; set; }
        [Required]
        [StringLength(1000)]
        public string Content { get; set; } = default!;
        [Required]
        [Range(1, 5)]
        public int Rating { get; set; }
        public bool IsDigital { get; set; }
        public DateTime ReviewDate { get; set; } = DateTime.Now;
    }
}
