
using Microsoft.EntityFrameworkCore;
using OnlineShop.Models;
using OnlineShop.Models.Enums;

namespace OnlineShop.Repositories.Repositories;

public class OrderExpirationRepository(SqlContext context): IOrderExpirationRepository
{
    public async Task<int> ExpirePendingOrdersAsync()
    {
        var now = DateTime.UtcNow;

        // سفارش‌های آنلاین منقضی‌شده
        var expiredOrderIds = await context.Orders
            .AsNoTracking()
            .Where(o =>
                o.PaymentMethod == PaymentMethod.Online &&
                o.Status == OrderStatus.PendingPayment &&
                o.PaymentStatus == PaymentStatus.Pending &&
                o.InventoryReservedUntil != null &&
                o.InventoryReservedUntil <= now &&
                o.InventoryReleasedAt == null)
            .Select(o => o.Id)
            .ToListAsync();

        var expiredCount = 0;

        foreach (var orderId in expiredOrderIds)
        {
            await using var transaction =
                await context.Database.BeginTransactionAsync();

            try
            {
                // فقط یک اجرای Job اجازه آزادسازی موجودی دارد
                var affectedRows = await context.Orders
                    .Where(o =>
                        o.Id == orderId &&
                        o.Status == OrderStatus.PendingPayment &&
                        o.PaymentStatus == PaymentStatus.Pending &&
                        o.InventoryReservedUntil != null &&
                        o.InventoryReservedUntil <= now &&
                        o.InventoryReleasedAt == null)
                    .ExecuteUpdateAsync(setters => setters
                        .SetProperty(
                            o => o.Status,
                            OrderStatus.Cancelled)
                        .SetProperty(
                            o => o.PaymentStatus,
                            PaymentStatus.Cancelled)
                        .SetProperty(
                            o => o.InventoryReleasedAt,
                            now)
                        .SetProperty(
                            o => o.CancelledAt,
                            now)
                        .SetProperty(
                            o => o.CancellationReason,
                            "مهلت پرداخت سفارش به پایان رسید."));

                // اگر قبلاً منقضی شده باشد، موجودی را برنمی‌گردانیم
                if (affectedRows == 0)
                {
                    await transaction.RollbackAsync();
                    continue;
                }

                // دریافت اقلام سفارش
                var items = await context.OrderItems
                    .AsNoTracking()
                    .Where(x => x.OrderId == orderId)
                    .ToListAsync();

                // بازگرداندن موجودی هر محصول
                foreach (var item in items)
                {
                    var updated = await context.Products
                        .Where(p => p.Id == item.ProductId)
                        .ExecuteUpdateAsync(setters => setters
                            .SetProperty(
                                p => p.Quantity,
                                p => p.Quantity + item.Quantity));

                    if (updated != 1)
                    {
                        throw new InvalidOperationException(
                            $"بازگرداندن موجودی محصول {item.ProductId} ناموفق بود.");
                    }
                }

                // لغو پرداخت‌های در انتظار
                await context.Payments
                    .Where(p =>
                        p.OrderId == orderId &&
                        p.Status == PaymentStatus.Pending)
                    .ExecuteUpdateAsync(setters => setters
                        .SetProperty(
                            p => p.Status,
                            PaymentStatus.Cancelled));

                await transaction.CommitAsync();

                expiredCount++;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        return expiredCount;
    }
}
