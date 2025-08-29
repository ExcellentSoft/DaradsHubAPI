using Microsoft.AspNetCore.Http;

namespace DaradsHubAPI.Core.Model.Request
{
    public class CreateCategoryRequestModel
    {
        public string Name { get; set; } = default!;
        public string? Description { get; set; }
        public int VendorId { get; set; }
        public IFormFile? Icon { get; set; }
    }

    public class UpdateCategoryRequestModel : CreateCategoryRequestModel
    {
        public int Id { get; set; }
    }

    public class CategoryResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public string? Description { get; set; }
        public string? Icon { get; set; }
    }
}
