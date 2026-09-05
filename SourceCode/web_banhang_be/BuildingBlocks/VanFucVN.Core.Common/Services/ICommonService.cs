namespace VanFucVN.Core.Common.Services;

public interface ICommonService
{
    void SetUserId(Guid userId);
    Guid GetUserId();
}
