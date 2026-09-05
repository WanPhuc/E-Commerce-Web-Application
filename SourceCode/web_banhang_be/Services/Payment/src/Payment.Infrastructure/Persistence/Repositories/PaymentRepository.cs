using Microsoft.EntityFrameworkCore;

namespace AuraMart.Payment.Infrastructure.Persistence.Repositories;

public class PaymentRepository : BaseRepository<Domain.Payment, PaymentDbContext>, IPaymentRepository
{
    public PaymentRepository(PaymentDbContext context) : base(context) { }

    public async Task<Domain.Payment?> GetByOrderIdAsync(Guid orderId)
    {
        return await _db.Payments
            .FirstOrDefaultAsync(p => p.OrderId == orderId && !p.DeleteFlg);
    }
}
