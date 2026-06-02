using Microsoft.EntityFrameworkCore;
using WebBanHang.Data;
using WebBanHang.Models.EntityModels;
using WebBanHang.Repositories.Interfaces;

namespace WebBanHang.Repositories.SqlServer;
public class NotificationRepository : BaseRepository<Notification>, INotificationRepository
{
    public NotificationRepository(AppDbContext context) : base(context)
    {
    }
   
}