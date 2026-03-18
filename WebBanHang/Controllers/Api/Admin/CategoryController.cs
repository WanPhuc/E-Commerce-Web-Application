using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebBanHang.Models.Common;

namespace WebBanHang.Controllers.Api.Admin;
[ApiController]
[Route("api/v1/admin/categories")]
[Authorize(Roles = "Admin")]
public class CategoryController  : ControllerBase
{
    private readonly ICategoryService _categoryService;
    public CategoryController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }
    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<CategoryDto>>>> GetAllCategories(CancellationToken ct = default)
    {
        var categories = await _categoryService.GetAllCategoriesAsync(ct);
        return Ok(ApiResponse<List<CategoryDto>>.Ok(categories));
    }
    [HttpGet("detail/{id:guid}")]
    public  async Task<ActionResult<ApiResponse<CategoryDto?>>> GetCategoryById(Guid id,CancellationToken ct = default)
    {
        try
        {
            var category =await _categoryService.GetCategoryByIdAsync(id,ct);
            return Ok(ApiResponse<CategoryDto?>.Ok(category));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<CategoryDto?>.Fail(ex.Message, 404));
        }
        catch (Exception)
        {
            return StatusCode(500, ApiResponse<CategoryDto?>.Fail("Internal server error", 500));
        }
    }
    [HttpPost("create")]
    public async Task<ActionResult<ApiResponse<CategoryDto>>> CreateCategory([FromBody] CreateCategoryDto dto,CancellationToken ct = default)
    {
        try
        {
            var category = await _categoryService.CreateCategoryAsync(dto,ct);
            return Ok(ApiResponse<CategoryDto>.Ok(category,"Category created successfully"));
        }
        catch(KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<CategoryDto>.Fail(ex.Message, 404));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<CategoryDto>.Fail(ex.Message, 400));
        }
        catch (Exception)
        {
            return StatusCode(500, ApiResponse<CategoryDto>.Fail("Internal server error", 500));
        }
    }
    [HttpPut("update/{id:guid}")]
    public async Task<ActionResult<ApiResponse<CategoryDto?>>> UpdateCategory(Guid id, [FromBody] UpdateCategoryDto dto,CancellationToken ct = default)
    {
        try
        {
            var category = await _categoryService.UpdateCategoryAsync(id, dto,ct);
            return Ok(ApiResponse<CategoryDto?>.Ok(category,"Category updated successfully"));
        }catch(KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<CategoryDto?>.Fail(ex.Message, 404));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<CategoryDto?>.Fail(ex.Message, 400));
        }
        catch (Exception)
        {
            return StatusCode(500, ApiResponse<CategoryDto?>.Fail("Internal server error", 500));
        }
    }
    [HttpDelete("delete/{id:guid}")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteCategory(Guid id,CancellationToken ct = default)
    {
        try
        {
            var result = await _categoryService.DeleteCategoryAsync(id,ct);
            return Ok(ApiResponse<bool>.Ok(result,"Category deleted successfully"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<bool>.Fail(ex.Message, 404));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<bool>.Fail(ex.Message, 400));
        }
        catch (Exception)
        {
            return StatusCode(500, ApiResponse<bool>.Fail("Internal server error", 500));
        }
    }


}