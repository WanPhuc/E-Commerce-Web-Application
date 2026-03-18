using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebBanHang.Core.Models;
namespace WebBanHang.Models;
public class ProductImage : Entity
{
    [Required(ErrorMessage = "URL hình ảnh là bắt buộc.")]
    [MaxLength(200, ErrorMessage = "URL hình ảnh không được vượt quá 200 ký tự.")]
    public required string ImageUrl { get; set; }

    public bool IsMainImage { get; set; } = false;

    [Required]
    public required Guid ProductId { get; set; }
    public Product Product { get; set; }= default!;

}