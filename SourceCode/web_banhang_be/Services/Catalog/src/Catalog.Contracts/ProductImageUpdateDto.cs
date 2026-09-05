namespace AuraMart.Catalog.Dtos;
public class ProductImageUpdateDto
{
    public string ImageUrl { get; set; } = default!;
    public bool IsMainImage { get; set; }
}
