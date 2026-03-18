using WebBanHang.Migrations;
using WebBanHang.Models;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;
    public CategoryService(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }
    public async Task<List<CategoryDto>> GetAllCategoriesAsync(CancellationToken ct = default)
    {
        var category =await _categoryRepository.GetAllWithChildrenAsync(ct);
        var dict = category.ToDictionary(x=>x.Id,x => new CategoryDto
        {
            Id=x.Id,
            Name=x.Name,
            ParentId=x.ParentId,
        });
        foreach(var item in dict.Values)
        {
            if(item.ParentId is Guid pid && dict.TryGetValue(pid,out var parent))
            parent.Children.Add(item);
        }

        var roots=dict.Values.Where(x=>x.ParentId == null).ToList();

        void SortRec(List<CategoryDto> nodes)
        {
            nodes.Sort((a, b) => string.Compare(a.Name, b.Name, StringComparison.OrdinalIgnoreCase));
            foreach (var n in nodes) SortRec(n.Children);
        }
        SortRec(roots);

        return roots;
    }
    public async Task<CategoryDto?> GetCategoryByIdAsync(Guid id, CancellationToken ct = default)
    {
        var all = await _categoryRepository.GetAllWithChildrenAsync(ct);
        var dict = all.ToDictionary(x=>x.Id,x=> new CategoryDto
        {
            Id=x.Id,
            Name=x.Name,
            ParentId=x.ParentId
        });

        foreach(var item in dict.Values)
        {
            if(item.ParentId is Guid pid && dict.TryGetValue(pid,out var parent))
            {
                parent.Children.Add(item);
            }
        }

        if(!dict.TryGetValue(id,out var root)){
            return null;
        }
        void SortRec(CategoryDto node)
        {
            node.Children.Sort((a,b)=>string.Compare(a.Name,b.Name,StringComparison.OrdinalIgnoreCase));
            foreach(var n in node.Children) SortRec(n);
        }
        SortRec(root);
        return root;
    }
    public async Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto dto,CancellationToken ct = default)
    {
        var name= dto.Name.Trim();

        if (dto.ParentId.HasValue)
        {
            var parentExists=await _categoryRepository.GetByIdWithChildrenAsync(dto.ParentId.Value);
            if(parentExists==null) throw new KeyNotFoundException("Parent category not found.");
            
        }
        if(await _categoryRepository.ExistsByNameAsync(name, dto.ParentId, ct))
        {
            throw new InvalidOperationException("Category with the same name already exists under the specified parent.");
        }
        var category = new Category
        {
            Name = name,
            ParentId = dto.ParentId
        };
        await _categoryRepository.AddAsync(category);
        await _categoryRepository.SaveChangesAsync();
        return new CategoryDto
        {
            Id=category.Id,
            Name=category.Name,
            ParentId=category.ParentId
        };
    }
    public async Task<CategoryDto?> UpdateCategoryAsync(Guid id, UpdateCategoryDto dto, CancellationToken ct = default)
    {
        var name = dto.Name.Trim();
        var category=await _categoryRepository.GetByIdWithChildrenAsync(id,ct);
        if(category == null) throw new KeyNotFoundException("Category not found.");

        if (dto.ParentId.HasValue)
        {
            var parentExists =await _categoryRepository.GetByIdWithChildrenAsync(dto.ParentId.Value,ct);
            if (parentExists == null) throw new KeyNotFoundException("Parent category not found.");
        }
        if(dto.ParentId == id) 
            throw new InvalidOperationException("A category cannot be its own parent.");

        if (dto.ParentId.HasValue)
        {
            var current = dto.ParentId.Value;
            while (true)
            {
                if(current == id)
                    throw new InvalidOperationException("A category cannot be a descendant of itself.");
                var p = await _categoryRepository.GetByIdWithChildrenAsync(current,ct);
                if (p == null || p.ParentId == null) break;
                current = p.ParentId.Value;
            }
        }

        if(await _categoryRepository.ExistsByNameExceptAsync(name, dto.ParentId, id, ct))
        {
            throw new InvalidOperationException("Category with the same name already exists under the specified parent.");
        }
        category.Name = name;
        category.ParentId = dto.ParentId;
        await _categoryRepository.UpdateAsync(category);
        await _categoryRepository.SaveChangesAsync();
        return new CategoryDto
        {
            Id=category.Id,
            Name=category.Name,
            ParentId=category.ParentId
        };
    }
    public async Task<bool> DeleteCategoryAsync(Guid id, CancellationToken ct = default)
    {
        var category = await _categoryRepository.GetByIdWithChildrenAsync(id,ct);
        if (category == null) throw new KeyNotFoundException("Category not found.");
        if (await _categoryRepository.HasChildrenAsync(id,ct))
        {
            throw new InvalidOperationException("Cannot delete a category that has sub-categories.");
        }
        await _categoryRepository.DeleteByIdAsync(id);
        await _categoryRepository.SaveChangesAsync();
        return true;
    }
}