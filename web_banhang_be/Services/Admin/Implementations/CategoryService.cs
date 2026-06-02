using WebBanHang.Extensions;
using WebBanHang.Migrations;
using WebBanHang.Models;
using WebBanHang.Models.Common;
using WebBanHang.Models.EntityModels;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;
    public CategoryService(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }
    public async Task<ApiResponse<PagedResult<CategoryDto>>> GetAllCategoriesAsync(PagedRequest request)
    {
        var category = await _categoryRepository.GetAllAsync();
        var dict = category.ToDictionary(x => x.Id, x => new CategoryDto
        {
            Id = x.Id,
            Name = x.Name,
            ParentId = x.ParentId,
        });
        foreach (var item in dict.Values)
        {
            if (item.ParentId is Guid pid && dict.TryGetValue(pid, out var parent))
                parent.Children.Add(item);
        }

        var roots = dict.Values.Where(x => x.ParentId == null).ToList();

        void SortRec(List<CategoryDto> nodes)
        {
            nodes.Sort((a, b) => string.Compare(a.Name, b.Name, StringComparison.OrdinalIgnoreCase));
            foreach (var n in nodes) SortRec(n.Children);
        }
        SortRec(roots);

        var pagedRoots = roots.Skip(request.Skip).Take(request.PageSize).ToList();
        var result = PagedResult<CategoryDto>.Create(pagedRoots, roots.Count, request);

        return ApiResponse<PagedResult<CategoryDto>>.Success(result, "Success", 200, SuccessCodes.Category.ListRetrieved);
    }
    public async Task<ApiResponse<CategoryDto?>> GetCategoryByIdAsync(Guid id)
    {
        var all = await _categoryRepository.GetAllAsync();
        var dict = all.ToDictionary(x => x.Id, x => new CategoryDto
        {
            Id = x.Id,
            Name = x.Name,
            ParentId = x.ParentId
        });

        foreach (var item in dict.Values)
        {
            if (item.ParentId is Guid pid && dict.TryGetValue(pid, out var parent))
            {
                parent.Children.Add(item);
            }
        }

        if (!dict.TryGetValue(id, out var root))
        {
            return ApiResponse<CategoryDto?>.Fail("Category not found.", 404, ErrorCodes.Category.NotFound);
        }
        void SortRec(CategoryDto node)
        {
            node.Children.Sort((a, b) => string.Compare(a.Name, b.Name, StringComparison.OrdinalIgnoreCase));
            foreach (var n in node.Children) SortRec(n);
        }
        SortRec(root);
        return ApiResponse<CategoryDto?>.Success(root, "Success", 200, SuccessCodes.Category.DetailRetrieved);
    }
    public async Task<ApiResponse<CategoryDto>> CreateCategoryAsync(CreateCategoryDto dto)
    {
        var name = dto.Name.Trim();

        if (dto.ParentId.HasValue)
        {
            var parentExists = await _categoryRepository.FindSingleByConditionAsync(c => c.Id == dto.ParentId.Value);
            if (parentExists == null) return ApiResponse<CategoryDto>.Fail("Parent category not found.", 404, ErrorCodes.Category.ParentNotFound);

        }
        if (await _categoryRepository.AnyByConditionAsync(c => c.Name == name && c.ParentId == dto.ParentId))
        {
            return ApiResponse<CategoryDto>.Fail("Category with the same name already exists under the specified parent.", 400, ErrorCodes.Category.DuplicateName);
        }
        var category = new Category
        {
            Name = name,
            ParentId = dto.ParentId
        };
        await _categoryRepository.CreateAsync(category);
        await _categoryRepository.SaveChangesAsync();
        var categoryDto = new CategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            ParentId = category.ParentId
        };
        return ApiResponse<CategoryDto>.Success(categoryDto, "Category created successfully", 201, SuccessCodes.Category.Created);
    }
    public async Task<ApiResponse<CategoryDto?>> UpdateCategoryAsync(Guid id, UpdateCategoryDto dto)
    {
        var name = dto.Name.Trim();
        var category = await _categoryRepository.FindSingleByConditionAsync(c => c.Id == id);
        if (category == null) return ApiResponse<CategoryDto?>.Fail("Category not found.", 404, ErrorCodes.Category.NotFound);

        if (dto.ParentId.HasValue)
        {
            var parentExists = await _categoryRepository.FindSingleByConditionAsync(c => c.Id == dto.ParentId.Value);
            if (parentExists == null) return ApiResponse<CategoryDto?>.Fail("Parent category not found.", 404, ErrorCodes.Category.ParentNotFound);
        }
        if (dto.ParentId == id)
            return ApiResponse<CategoryDto?>.Fail("A category cannot be its own parent.", 400, ErrorCodes.Category.InvalidParent);

        if (dto.ParentId.HasValue)
        {
            var current = dto.ParentId.Value;
            while (true)
            {
                if (current == id)
                    return ApiResponse<CategoryDto?>.Fail("A category cannot be a descendant of itself.", 400, ErrorCodes.Category.InvalidParent);
                var p = await _categoryRepository.FindSingleByConditionAsync(c => c.Id == current);
                if (p == null || p.ParentId == null) break;
                current = p.ParentId.Value;
            }
        }

        if (await _categoryRepository.AnyByConditionAsync(c => c.Name == name && c.ParentId == dto.ParentId && c.Id != id))
        {
            return ApiResponse<CategoryDto?>.Fail("Category with the same name already exists under the specified parent.", 400, ErrorCodes.Category.DuplicateName);
        }
        category.Name = name;
        category.ParentId = dto.ParentId;
        await _categoryRepository.UpdateAsync(category);
        await _categoryRepository.SaveChangesAsync();
        var categoryDto = new CategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            ParentId = category.ParentId
        };
        return ApiResponse<CategoryDto?>.Success(categoryDto, "Category updated successfully", 200, SuccessCodes.Category.Updated);
    }
    public async Task<ApiResponse<bool>> DeleteCategoryAsync(Guid id)
    {
        var category = await _categoryRepository.FindSingleByConditionAsync(c => c.Id == id);
        if (category == null) return ApiResponse<bool>.Fail("Category not found.", 404, ErrorCodes.Category.NotFound);
        if (await _categoryRepository.AnyByConditionAsync(c => c.ParentId == id))
        {
            return ApiResponse<bool>.Fail("Cannot delete a category that has sub-categories.", 400, ErrorCodes.Category.CannotDeleteHasChildren);
        }
        await _categoryRepository.SoftDeleteAsync(id);
        await _categoryRepository.SaveChangesAsync();
        return ApiResponse<bool>.Success(true, "Category deleted successfully", 200, SuccessCodes.Category.Deleted);
    }
}
