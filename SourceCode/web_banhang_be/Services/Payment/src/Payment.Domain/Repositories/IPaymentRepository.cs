namespace AuraMart.Payment.Domain.Repositories;

public interface IPaymentRepository : IBaseRepository<Payment>
{
    Task<Payment?> GetByOrderIdAsync(Guid orderId);
}
