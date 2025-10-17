using Microsoft.AspNetCore.Http;

namespace DaradsHubAPI.Core.Model.Request
{
    public class CreateCategoryRequestModel
    {
        public string Name { get; set; } = default!;
        public string? Description { get; set; }
        public IFormFile? Icon { get; set; }
    }
    public class CreateSubCategoryRequestModel
    {
        public int CategoryId { get; set; } = default!;
        public IEnumerable<string> SubCategoryNames { get; set; } = default!;
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
        public IEnumerable<SubCategoryDatum> SubCategoryData { get; set; } = default!;
    }
    public class SubCategoryDatum
    {
        public long Id { get; set; }
        public string Name { get; set; } = default!;
    }
}
