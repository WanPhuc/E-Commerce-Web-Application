using WebBanHang.Services.Global.Interfaces;

namespace WebBanHang.Services.Global.Implements
{
    public class CommonService:ICommonService
    {
        private Guid userId;
        public void SetUserId(Guid userId)
        {
            this.userId = userId;
        }
        public Guid GetUserId()
        {
            return this.userId;
        }
    }
}
