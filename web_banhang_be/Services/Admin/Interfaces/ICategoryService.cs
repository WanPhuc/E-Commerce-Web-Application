using WebBanHang.Models.Common;

public interface ICategoryService
{
    Task<ApiResponse<PagedResult<CategoryDto>>> GetAllCategoriesAsync(PagedRequest request);
    Task<ApiResponse<CategoryDto?>> GetCategoryByIdAsync(Guid id);
    Task<ApiResponse<CategoryDto>> CreateCategoryAsync(CreateCategoryDto dto );
    Task<ApiResponse<CategoryDto?>> UpdateCategoryAsync(Guid id, UpdateCategoryDto dto);
    Task<ApiResponse<bool>> DeleteCategoryAsync(Guid id);
}
