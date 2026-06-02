namespace WebBanHang.Models.Common
{
    public class PagedRequest
    {
        private int _page =1 ;
        private int _pageSize = 10 ;

        // Trang hien tai , mac dinh la 1 , toi thieu 1
        public int Page
        {
            get => _page;
            set => _page = value < 1 ? 1 : value;
        }
        //so luong record tren moi trang , mac dinh la 10 , toi da 100
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = value < 1 ? 1 : value > 100 ? 100 : value;
        }

        //so Record bo qua , VD: page =3 , pagesize =10 -> skip =  20
        public int Skip => (Page - 1) * PageSize;
    }
}
