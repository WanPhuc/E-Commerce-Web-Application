using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebBanHang.Controllers.Api.Base;
using WebBanHang.Filters;
using WebBanHang.Models.Common;

namespace WebBanHang.Controllers.Api.Admin;
[ApiController]
[Route("api/v1/admin/categories")]
[AuthorizeRole( "Admin")]
public class CategoryController  : BaseController
{
    private readonly ICategoryService _categoryService;
    public CategoryController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }
    [HttpGet]
    public async Task<IActionResult> GetAllCategories([FromQuery] PagedRequest request)
    {
        var categories = await _categoryService.GetAllCategoriesAsync(request);
        return BaseResult(categories);
    }
    [HttpGet("detail/{id:guid}")]
    public async Task<IActionResult> GetCategoryById(Guid id)
    {
        var category = await _categoryService.GetCategoryByIdAsync(id);
        return BaseResult(category);
    }
    [HttpPost("create")]
    public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryDto dto)
    {
        var category = await _categoryService.CreateCategoryAsync(dto);
        return BaseResult(category);
    }
    [HttpPut("update/{id:guid}")]
    public async Task<IActionResult> UpdateCategory(Guid id, [FromBody] UpdateCategoryDto dto)
    {
        var category = await _categoryService.UpdateCategoryAsync(id, dto);
        return BaseResult(category);
    }
    [HttpDelete("delete/{id:guid}")]
    public async Task<IActionResult> DeleteCategory(Guid id)
    {
        var result = await _categoryService.DeleteCategoryAsync(id);
        return BaseResult(result);
    }


}
