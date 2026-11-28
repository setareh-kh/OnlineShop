using OnlineShop.Models;
using OnlineShop.Models.Enums;

namespace OnlineShop.Repositories;

public interface IPaymentRepository : IBaseRepository<Payment>
{
    Task<Payment?> GetByOrderIdAsync(int orderId);

    Task<Payment?> GetByAuthorityAsync(string authority);

    Task<PaymentFinalizationResult> CompletePaymentAsync(
        int paymentId,
        int orderId,
        string authority,
        string transactionId,
        DateTime paidAt);
}