public class AdminProductQuery
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public ProductStatus? Status { get; set; }
    public Guid? CategoryId { get; set; }
    public Guid? SellerId { get; set; }
    public string? Keyword { get; set; }

    public string SortBy{ get; set; } = "createdAt";
    public bool SortDesc { get; set; } = true;

}