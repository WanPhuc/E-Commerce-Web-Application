using WebBanHang.Models.Common;

public class PagedResult<T>
{
    public List<T> Data { get; set; }=new();
    public int Page{get;set;}
    public int PageSize{get;set;}
    public int TotalRecords { get; set; }
    //tong trang = tong record chia cho page size , lam tron len
    public int TotalPages => (int)Math.Ceiling((double)TotalRecords / PageSize);
    // Con trang tiep không
    public bool HasNextPage => Page < TotalPages;
    // Con trang truoc khong
    public bool HasPreviousPage => Page > 1;

    // ham tao nhanh pageresult thay vi phai New thu cong nhieu lan
    public static PagedResult<T> Create(List<T> data,int totalRecords, PagedRequest request)
    {
        return new PagedResult<T>
        {
            Data = data,
            Page = request.Page,
            PageSize = request.PageSize,
            TotalRecords = totalRecords
        };
    }
}