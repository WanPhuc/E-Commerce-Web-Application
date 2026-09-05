namespace AuraMart.Catalog.Dtos;
public class ProductImageCreateDto
{
    public string ImageUrl { get; set; } = default!;
    public bool IsMainImage { get; set; }
}
