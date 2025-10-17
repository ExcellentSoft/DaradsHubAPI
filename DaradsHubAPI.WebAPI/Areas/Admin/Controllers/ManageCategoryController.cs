using DaradsHubAPI.Core.Model;
using DaradsHubAPI.Core.Model.Request;
using DaradsHubAPI.Core.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace DaradsHubAPI.WebAPI.Areas.Admin.Controllers;
[Tags("Admin")]
public class ManageCategoryController(ICategoryService _categoryService) : ApiBaseController
{

    [HttpPost("add-category")]
    [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> CreateCategory([FromForm] CreateCategoryRequestModel request)
    {
        var response = await _categoryService.CreateCategory(request);
        return ResponseCode(response);
    }

    [HttpPatch("update-category")]
    [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> UpdateCategory([FromForm] UpdateCategoryRequestModel request)
    {
        var response = await _categoryService.UpdateCategory(request);
        return ResponseCode(response);
    }

    [HttpPost("add-update-sub-category")]
    [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> CreateUpdateSubCategory([FromBody] CreateSubCategoryRequestModel request)
    {
        var response = await _categoryService.CreateUpdateSubCategory(request);
        return ResponseCode(response);
    }

    [HttpGet("category")]
    [ProducesResponseType(typeof(ApiResponse<CategoryResponse>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetById([FromQuery] int categoryId)
    {
        var response = await _categoryService.GetById(categoryId);
        return ResponseCode(response);
    }

    [HttpDelete("delete-category")]
    [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> DeleteCategory([FromQuery] int categoryId)
    {
        var response = await _categoryService.DeleteCategory(categoryId);
        return ResponseCode(response);
    }

    [HttpDelete("delete-sub-category")]
    [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> DeleteSubCategory([FromQuery] int subCategoryId)
    {
        var response = await _categoryService.DeleteSubCategory(subCategoryId);
        return ResponseCode(response);
    }
}