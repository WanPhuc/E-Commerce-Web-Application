using System.ComponentModel.DataAnnotations;
using WebBanHang.Core.Models;
namespace WebBanHang.Models;
public class Category : Entity
{
    [Required(ErrorMessage = "Tên danh mục là bắt buộc.")]
    [MaxLength(100, ErrorMessage = "Tên danh mục không được vượt quá 100 ký tự.")]
    [Display(Name = "Tên danh mục")]
    public required string Name { get; set; }

    public Guid? ParentId { get; set; }
    public Category? Parent { get; set; }

    public ICollection<Category> SubCategories { get; set; }= new List<Category>();
    public ICollection<Product> Products { get; set; }= new List<Product>();

}