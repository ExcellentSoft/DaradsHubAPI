namespace DaradsHubAPI.Core.Model.Response
{
    public class ProductDetailsResponse
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public List<ImageDto>? Images { get; set; }
        public int CategoryId { get; set; }
        public List<string>? Colors { get; set; }
        public List<string>? Sizes { get; set; }
        public int Stock { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.Now;
        public DateTime DateUpdated { get; set; }
        public bool IsDeleted { get; set; }
        public int VendorId { get; set; }
        public string? VendorName { get; set; }
        public string CategoryName { get; set; }
        // public ICollection<Review>? Reviews { get; set; } = new List<Review>();
    }
}
