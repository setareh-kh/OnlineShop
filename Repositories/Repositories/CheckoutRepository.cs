using Microsoft.EntityFrameworkCore;
using OnlineShop.Dtos.Objects;
using OnlineShop.Dtos.Requests;
using OnlineShop.Models;
using OnlineShop.Models.Enums;

namespace OnlineShop.Repositories.Repositories;

public class CheckoutRepository(SqlContext context) : ICheckoutRepository
{
    public async Task<CheckoutCreateResult> CreateOrderAsync(int userId,
        CheckoutRequest request,
        string idempotencyKey,
        decimal shippingCost,
        int reservationMinutes)
    {
        // 1. بررسی درخواست تکراری
        var existingOrder = await FindExistingOrderAsync(userId, idempotencyKey);

        if (existingOrder != null)
            return new CheckoutCreateResult
            {
                Success = true,
                Order = existingOrder
            };

        // 2. اعتبارسنجی روش ارسال و پرداخت
        if (request.ShippingMethod != ShippingMethod.Post)
            return Fail("روش ارسال پشتیبانی نمی‌شود.");

        if (request.PaymentMethod != PaymentMethod.Online &&
            request.PaymentMethod != PaymentMethod.CashOnDelivery)
            return Fail("روش پرداخت نامعتبر است.");

        if (shippingCost < 0 || reservationMinutes <= 0)
            return Fail("تنظیمات Checkout نامعتبر است.");

        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            // 3. بررسی مجدد درخواست تکراری
            existingOrder = await FindExistingOrderAsync(userId, idempotencyKey);

            if (existingOrder != null)
            {
                await transaction.RollbackAsync();

                return new CheckoutCreateResult
                {
                    Success = true,
                    Order = existingOrder
                };
            }

            // 4. دریافت سبد خرید و محصولات
            var cart = await context.Carts
                .Include(x => x.Items)
                .ThenInclude(x => x.Product)
                .FirstOrDefaultAsync(x => x.UserId == userId);

            if (cart == null || cart.Items.Count == 0)
                return Fail("سبد خرید خالی است.");

            // 5. اعتبارسنجی تمام محصولات قبل از رزرو
            foreach (var item in cart.Items)
            {
                var product = item.Product;

                if (item.Quantity <= 0)
                    return Fail("تعداد محصول نامعتبر است.");

                if (!product.Active || !product.Available)
                    return Fail($"محصول {product.Name} در دسترس نیست.");

                if (product.Price < 0 ||
                    product.Discount is < 0 or > 100)
                    return Fail($"قیمت یا تخفیف {product.Name} نامعتبر است.");

                if (product.Quantity < item.Quantity)
                    return Fail($"موجودی {product.Name} کافی نیست.");
            }

            // 6. دریافت تصاویر اصلی محصولات
            var productIds = cart.Items
                .Select(x => x.ProductId)
                .Distinct()
                .ToList();

            var images = await context.Images
                .Where(x =>
                    x.ItemType == "Product" &&
                    productIds.Contains(x.ItemId))
                .GroupBy(x => x.ItemId)
                .Select(g => g
                    .OrderBy(x => x.Priority)
                    .ThenBy(x => x.Id)
                    .First())
                .ToDictionaryAsync(x => x.ItemId, x => x.Url);

            // 7. محاسبه مبالغ از دیتابیس
            decimal subTotal = 0;
            decimal discountTotal = 0;

            var orderItems = new List<OrderItem>();

            foreach (var item in cart.Items)
            {
                var product = item.Product;

                var finalUnitPrice = Math.Round(
                    product.Price * (100 - product.Discount) / 100,
                    2,
                    MidpointRounding.AwayFromZero);

                var discountPerUnit =
                    product.Price - finalUnitPrice;

                var discountAmount =
                    discountPerUnit * item.Quantity;

                var itemTotal =
                    finalUnitPrice * item.Quantity;

                images.TryGetValue(product.Id, out var imageUrl);

                orderItems.Add(new OrderItem
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    ImageUrl = imageUrl,
                    UnitPrice = product.Price,
                    DiscountPercent = product.Discount,
                    FinalUnitPrice = finalUnitPrice,
                    Quantity = item.Quantity,
                    DiscountAmount = discountAmount,
                    TotalPrice = itemTotal,
                    CreateAt = DateTime.UtcNow
                });

                subTotal += product.Price * item.Quantity;
                discountTotal += discountAmount;
            }

            subTotal = Math.Round(
                subTotal, 2, MidpointRounding.AwayFromZero);

            discountTotal = Math.Round(
                discountTotal, 2, MidpointRounding.AwayFromZero);

            var totalPrice = Math.Round(
                subTotal - discountTotal + shippingCost,
                2,
                MidpointRounding.AwayFromZero);

            // 8. رزرو اتمیک موجودی
            foreach (var item in cart.Items)
            {
                var affectedRows = await context.Products
                    .Where(p =>
                        p.Id == item.ProductId &&
                        p.Active &&
                        p.Available &&
                        p.Quantity >= item.Quantity)
                    .ExecuteUpdateAsync(setters =>
                        setters.SetProperty(
                            p => p.Quantity,
                            p => p.Quantity - item.Quantity));

                if (affectedRows != 1)
                    return Fail(
                        $"موجودی محصول {item.Product.Name} تغییر کرده است. دوباره تلاش کنید.");
            }

            // 9. ایجاد سفارش
            var now = DateTime.UtcNow;

            var isOnline = request.PaymentMethod == PaymentMethod.Online;

            var order = new Order
            {
                OrderNumber = $"ORD-{Guid.NewGuid():N}"[..20].ToUpperInvariant(),

                IdempotencyKey = idempotencyKey,
                UserId = userId,

                Status = isOnline ? OrderStatus.PendingPayment : OrderStatus.Confirmed,

                PaymentStatus = PaymentStatus.Pending,
                PaymentMethod = request.PaymentMethod,
                ShippingMethod = request.ShippingMethod,

                SubTotal = subTotal,
                DiscountTotal = discountTotal,
                ShippingCost = shippingCost,
                TotalPrice = totalPrice,

                RecipientName = request.RecipientName,
                RecipientMobile = request.RecipientMobile,
                Province = request.Province,
                City = request.City,
                Address = request.Address,
                PostalCode = request.PostalCode,
                Description = request.Description,

                InventoryReservedAt = now,

                InventoryReservedUntil = isOnline ? now.AddMinutes(reservationMinutes) : null,

                CreateAt = now,
                Items = orderItems
            };

            // 10. ایجاد Payment اولیه
            var payment = new Payment
            {
                Order = order,
                Amount = totalPrice,
                Method = request.PaymentMethod,
                Status = PaymentStatus.Pending,
                CreateAt = now
            };

            order.Payments.Add(payment);

            context.Orders.Add(order);

            // 11. پاک کردن سبد خرید
            context.CartItems.RemoveRange(cart.Items);

            cart.UpdateDate = now;

            // 12. ذخیره همه تغییرات و Commit
            await context.SaveChangesAsync();

            await transaction.CommitAsync();

            return new CheckoutCreateResult
            {
                Success = true,
                Order = order
            };
        }
        catch (DbUpdateException)
        {
            await transaction.RollbackAsync();

            // احتمال درخواست هم‌زمان با IdempotencyKey یکسان
            existingOrder = await FindExistingOrderAsync(userId, idempotencyKey);

            if (existingOrder != null)
            {
                return new CheckoutCreateResult
                {
                    Success = true,
                    Order = existingOrder
                };
            }

            throw;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    private async Task<Order?> FindExistingOrderAsync(int userId, string idempotencyKey)
    {
        return await context.Orders.AsNoTracking().FirstOrDefaultAsync(x => x.UserId == userId &&
                x.IdempotencyKey == idempotencyKey);
    }

    private static CheckoutCreateResult Fail(string message)
    {
        return new CheckoutCreateResult
        {
            Success = false,
            Message = message
        };
    }
}
