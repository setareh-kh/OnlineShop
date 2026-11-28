
using Microsoft.EntityFrameworkCore;
using OnlineShop.Models;
using OnlineShop.Models.Enums;

namespace OnlineShop.Repositories.Repositories;

public class PaymentRepository(SqlContext sqlContext)
    : BaseRepository<Payment>(sqlContext), IPaymentRepository
{
    public async Task<Payment?> GetByOrderIdAsync(int orderId)
    {
        return await SqlContext.Payments
            .Include(p => p.Order)
            .FirstOrDefaultAsync(p => p.OrderId == orderId);
    }

    public async Task<Payment?> GetByAuthorityAsync(string authority)
    {
        return await SqlContext.Payments
            .Include(p => p.Order)
            .FirstOrDefaultAsync(p => p.Authority == authority);
    }

    public async Task<PaymentFinalizationResult> CompletePaymentAsync(
    int paymentId,
    int orderId,
    string authority,
    string transactionId,
    DateTime paidAt)
{
    await using var transaction =
        await SqlContext.Database.BeginTransactionAsync();

    try
    {
        // تلاش اول: تأیید عادی سفارش
        var orderAffected = await SqlContext.Orders
            .Where(o =>
                o.Id == orderId &&
                o.PaymentMethod == PaymentMethod.Online &&
                o.Status == OrderStatus.PendingPayment &&
                o.PaymentStatus == PaymentStatus.Pending &&
                o.InventoryReleasedAt == null &&
                o.InventoryReservedUntil != null &&
                o.InventoryReservedUntil > paidAt)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(o => o.Status, OrderStatus.Confirmed)
                .SetProperty(o => o.PaymentStatus, PaymentStatus.Successful)
                .SetProperty(o => o.UpdateDate, paidAt));

        if (orderAffected == 1)
        {
            var paymentAffected = await SqlContext.Payments
                .Where(p =>
                    p.Id == paymentId &&
                    p.OrderId == orderId &&
                    p.Method == PaymentMethod.Online &&
                    p.Status == PaymentStatus.Pending &&
                    p.Authority == authority)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(p => p.Status, PaymentStatus.Successful)
                    .SetProperty(p => p.TransactionId, transactionId)
                    .SetProperty(p => p.PaidAt, paidAt)
                    .SetProperty(p => p.UpdateDate, paidAt));

            if (paymentAffected != 1)
            {
                await transaction.RollbackAsync();
                return PaymentFinalizationResult.NotCompleted;
            }

            await transaction.CommitAsync();

            return PaymentFinalizationResult.Completed;
        }

        // تلاش دوم: سفارش قبلاً لغو و موجودی آزاد شده است.
        // در این حالت پول دریافت‌شده را ثبت می‌کنیم،
        // اما وضعیت سفارش را از Cancelled به Confirmed تغییر نمی‌دهیم.

        var cancelledOrderAffected = await SqlContext.Orders
            .Where(o =>
                o.Id == orderId &&
                o.Status == OrderStatus.Cancelled &&
                o.PaymentStatus == PaymentStatus.Cancelled &&
                o.InventoryReleasedAt != null)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(o => o.PaymentStatus, PaymentStatus.Successful)
                .SetProperty(o => o.UpdateDate, paidAt));

        if (cancelledOrderAffected != 1)
        {
            await transaction.RollbackAsync();
            return PaymentFinalizationResult.NotCompleted;
        }

        var latePaymentAffected = await SqlContext.Payments
            .Where(p =>
                p.Id == paymentId &&
                p.OrderId == orderId &&
                p.Method == PaymentMethod.Online &&
                (p.Status == PaymentStatus.Pending ||
                 p.Status == PaymentStatus.Cancelled) &&
                p.Authority == authority)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(p => p.Status, PaymentStatus.Successful)
                .SetProperty(p => p.TransactionId, transactionId)
                .SetProperty(p => p.PaidAt, paidAt)
                .SetProperty(p => p.UpdateDate, paidAt));

        if (latePaymentAffected != 1)
        {
            await transaction.RollbackAsync();
            return PaymentFinalizationResult.NotCompleted;
        }

        await transaction.CommitAsync();

        return PaymentFinalizationResult.LatePaymentRecorded;
    }
    catch
    {
        await transaction.RollbackAsync();
        throw;
    }
}
}