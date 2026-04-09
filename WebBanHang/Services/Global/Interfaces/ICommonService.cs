namespace WebBanHang.Services.Global.Interfaces
{
    public interface ICommonService
    {
        void SetUserId(Guid userId);
        Guid GetUserId();
    }
}
