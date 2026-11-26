using Microsoft.Extensions.Options;
using OnlineShop.Dtos.Objects;
using OnlineShop.Dtos.Responses;
using OnlineShop.Repositories;
using OnlineShop.Dtos.Requests;
using OnlineShop.Models.Enums;
namespace OnlineShop.Services.Services;

public class CheckoutService(
    ICartRepository cartRepository,
    IImageService imageService,
    IOptions<CheckoutSettings> options,
    ICheckoutRepository checkoutRepository) : ICheckoutService
{
    private readonly decimal shippingCost = options.Value.PostShippingCost;
    private readonly int reservationMinutes = options.Value.OnlinePaymentReservationMinutes;
    public async Task<CheckoutPreviewResponse> PreviewAsync(int userId)
    {
        var response = new CheckoutPreviewResponse
        {
            ShippingCost = shippingCost
        };

        var cart = await cartRepository.GetCartWithItemsAsync(userId);

        if (cart == null || cart.Items.Count == 0)
        {
            response.CanCheckout = false;
            response.Warnings.Add("سبد خرید شما خالی است.");
            response.ShippingCost = 0;
            return response;
        }

        if (shippingCost < 0)
        {
            response.CanCheckout = false;
            response.Warnings.Add("هزینه ارسال نامعتبر است.");
            response.ShippingCost = 0;
        }

        var productIds = cart.Items.Select(x => x.ProductId).Distinct().ToList();

        var images = await imageService.GetMainProductImagesAsync(productIds);

        var imageDictionary = images.ToDictionary(x => x.ItemId);

        foreach (var cartItem in cart.Items)
        {
            var product = cartItem.Product;

            imageDictionary.TryGetValue(product.Id, out var image);

            var validPrice = product.Price >= 0;
            var validDiscount = product.Discount is >= 0 and <= 100;

            var discountPercent = Math.Clamp(product.Discount, 0, 100);

            var finalUnitPrice = Math.Round(product.Price * (100 - discountPercent) / 100, 2, MidpointRounding.AwayFromZero);

            var discountAmount = product.Price - finalUnitPrice;

            var hasEnoughStock =
                product.Active &&
                product.Available &&
                product.Quantity >= cartItem.Quantity &&
                cartItem.Quantity > 0 &&
                validPrice &&
                validDiscount;

            if (!product.Active)
                response.Warnings.Add($"محصول «{product.Name}» غیرفعال است.");

            if (!product.Available)
                response.Warnings.Add($"محصول «{product.Name}» در دسترس نیست.");

            if (cartItem.Quantity <= 0 ||
                cartItem.Quantity > product.Quantity)
            {
                response.Warnings.Add(
                    $"موجودی محصول «{product.Name}» برای تعداد درخواستی کافی نیست.");
            }

            if (!validPrice)
                response.Warnings.Add($"قیمت محصول «{product.Name}» نامعتبر است.");

            if (!validDiscount)
                response.Warnings.Add($"تخفیف محصول «{product.Name}» نامعتبر است.");

            response.Items.Add(new CheckoutItemResponse
            {
                ProductId = product.Id,
                ProductName = product.Name,
                ImageUrl = image?.Url,
                UnitPrice = product.Price,
                DiscountPercent = product.Discount,
                FinalUnitPrice = finalUnitPrice,
                Quantity = cartItem.Quantity,
                AvailableStock = product.Quantity,
                DiscountAmount = discountAmount * cartItem.Quantity,
                TotalPrice = finalUnitPrice * cartItem.Quantity,
                HasEnoughStock = hasEnoughStock
            });

            response.SubTotal += product.Price * cartItem.Quantity;

            response.DiscountTotal += discountAmount * cartItem.Quantity;
        }

        response.SubTotal = Math.Round(response.SubTotal, 2, MidpointRounding.AwayFromZero);

        response.DiscountTotal = Math.Round(response.DiscountTotal, 2, MidpointRounding.AwayFromZero);

        response.TotalPrice = Math.Round(response.SubTotal - response.DiscountTotal + response.ShippingCost, 2, MidpointRounding.AwayFromZero);

        response.CanCheckout = response.Items.All(x => x.HasEnoughStock) && shippingCost >= 0;

        return response;
    }
    public async Task<CheckoutResponse> CreateOrderAsync(int userId, CheckoutRequest request, string idempotencyKey)
    {
        // 1. اعتبارسنجی ورودی‌ها
        ArgumentNullException.ThrowIfNull(request);

        if (userId <= 0)
            throw new ArgumentException("شناسه کاربر نامعتبر است.");

        if (string.IsNullOrWhiteSpace(idempotencyKey) ||
            idempotencyKey.Length > 100)
        {
            throw new ArgumentException("کلید Idempotency نامعتبر است.");
        }

        // 2. اعتبارسنجی روش پرداخت
        if (request.PaymentMethod != PaymentMethod.Online &&
            request.PaymentMethod != PaymentMethod.CashOnDelivery)
        {
            throw new ArgumentException("روش پرداخت نامعتبر است.");
        }

        // 3. اعتبارسنجی روش ارسال
        if (request.ShippingMethod != ShippingMethod.Post)
        {
            throw new ArgumentException("روش ارسال نامعتبر است.");
        }

        // 4. بررسی تنظیمات ارسال و رزرو موجودی
        if (shippingCost < 0)
            throw new InvalidOperationException("هزینه ارسال نامعتبر است.");

        if (reservationMinutes <= 0)
            throw new InvalidOperationException("زمان رزرو موجودی نامعتبر است.");

        // 5. درخواست ایجاد سفارش از Repository
        var result = await checkoutRepository.CreateOrderAsync(userId,request,idempotencyKey,shippingCost,reservationMinutes);

        // 6. بررسی نتیجه عملیات Repository
        if (!result.Success || result.Order == null)
        {
            throw new InvalidOperationException(
                result.Message ?? "ایجاد سفارش با خطا مواجه شد.");
        }

        var order = result.Order;

        // 7. تبدیل سفارش به CheckoutResponse
        return new CheckoutResponse
        {
            OrderId = order.Id,
            OrderNumber = order.OrderNumber,
            Status = order.Status,
            PaymentStatus = order.PaymentStatus,
            PaymentMethod = order.PaymentMethod,
            ShippingMethod = order.ShippingMethod,
            TotalPrice = order.TotalPrice,
            CreateAt = order.CreateAt
        };
    }
}