using OnlineShop.Dtos.Objects;
using OnlineShop.Models;
using OnlineShop.Models.Enums;
using OnlineShop.Repositories;

namespace OnlineShop.Services.Services;

public class PaymentService(
    IPaymentRepository repository,
    IPaymentGateway gateway)
    : BaseService<Payment>(repository), IPaymentService
{
    private const string GatewayName = "Zarinpal";

    public async Task<PaymentGatewayResult> StartPaymentAsync(
        int orderId,
        int userId,
        string callbackUrl)
    {
        var payment = await repository.GetByOrderIdAsync(orderId);

        if (payment == null)
        {
            return PaymentError("پرداخت این سفارش پیدا نشد.");
        }

        var order = payment.Order;

        // بررسی مالکیت سفارش
        if (order.UserId != userId)
        {
            return PaymentError("شما اجازه پرداخت این سفارش را ندارید.");
        }

        // بررسی روش پرداخت
        if (payment.Method != PaymentMethod.Online ||
            order.PaymentMethod != PaymentMethod.Online)
        {
            return PaymentError("روش پرداخت این سفارش آنلاین نیست.");
        }

        // بررسی وضعیت سفارش و پرداخت
        if (order.Status != OrderStatus.PendingPayment ||
            order.PaymentStatus != PaymentStatus.Pending ||
            payment.Status != PaymentStatus.Pending)
        {
            return PaymentError("این سفارش در وضعیت قابل پرداخت نیست.");
        }

        // بررسی مهلت رزرو موجودی
        if (order.InventoryReservedUntil == null ||
            order.InventoryReservedUntil <= DateTime.UtcNow ||
            order.InventoryReleasedAt != null)
        {
            return PaymentError("مهلت پرداخت سفارش به پایان رسیده است.");
        }

        // کنترل مبلغ پرداخت
        if (payment.Amount <= 0 ||
            payment.Amount != order.TotalPrice)
        {
            return PaymentError("مبلغ پرداخت با مبلغ سفارش مطابقت ندارد.");
        }

        // اگر قبلاً درخواست پرداخت ثبت شده باشد،
        // همان Authority و لینک قبلی را برمی‌گردانیم.
        if (!string.IsNullOrWhiteSpace(payment.Authority))
        {
            if (payment.Gateway != GatewayName)
            {
                return PaymentError("درگاه پرداخت قبلی با درگاه فعلی متفاوت است.");
            }

            return new PaymentGatewayResult
            {
                Success = true,
                Authority = payment.Authority,
                PaymentUrl = gateway.GetPaymentUrl(payment.Authority)
            };
        }

        var gatewayResult = await gateway.CreatePaymentAsync(
            new PaymentGatewayRequest
            {
                AmountToman = payment.Amount,
                Description = $"پرداخت سفارش {order.OrderNumber}",
                CallbackUrl = callbackUrl,
                Mobile = order.RecipientMobile
            });

        if (!gatewayResult.Success ||
            string.IsNullOrWhiteSpace(gatewayResult.Authority))
        {
            return gatewayResult;
        }

        payment.Gateway = GatewayName;
        payment.Authority = gatewayResult.Authority;
        payment.UpdateDate = DateTime.UtcNow;

        var saved = await repository.SaveChangesAsync();

        if (!saved)
        {
            return PaymentError(
                "درخواست پرداخت ایجاد شد، اما ذخیره اطلاعات آن در فروشگاه ناموفق بود.");
        }

        return gatewayResult;
    }

    public async Task<PaymentGatewayVerifyResult> VerifyPaymentAsync(
    string authority,
    string status)
{
    if (string.IsNullOrWhiteSpace(authority))
    {
        return VerifyError("شناسه پرداخت معتبر نیست.");
    }

    if (!string.Equals(status, "OK", StringComparison.OrdinalIgnoreCase))
    {
        return VerifyError("پرداخت توسط کاربر انجام نشد یا لغو شد.");
    }

    var payment = await repository.GetByAuthorityAsync(authority);

    if (payment == null)
    {
        return VerifyError("پرداخت موردنظر پیدا نشد.");
    }

    var order = payment.Order;

    if (payment.Gateway != GatewayName)
    {
        return VerifyError("درگاه این پرداخت معتبر نیست.");
    }

    if (payment.Method != PaymentMethod.Online ||
        order.PaymentMethod != PaymentMethod.Online)
    {
        return VerifyError("روش پرداخت این سفارش آنلاین نیست.");
    }

    // Callback تکراری برای پرداختی که قبلاً در سیستم ثبت شده
    if (payment.Status == PaymentStatus.Successful &&
        order.PaymentStatus == PaymentStatus.Successful)
    {
        return new PaymentGatewayVerifyResult
        {
            Success = true,
            AlreadyVerified = true,
            OrderConfirmed = order.Status != OrderStatus.Cancelled,
            TransactionId = payment.TransactionId
        };
    }

    // بررسی نهایی تراکنش در خود درگاه
    var verifyResult = await gateway.VerifyPaymentAsync(
        new PaymentGatewayVerifyRequest
        {
            Authority = authority,
            AmountToman = payment.Amount
        });

    if (!verifyResult.Success)
    {
        return verifyResult;
    }

    if (string.IsNullOrWhiteSpace(verifyResult.TransactionId))
    {
        return new PaymentGatewayVerifyResult
        {
            Success = true,
            OrderConfirmed = false,
            ErrorMessage =
                "درگاه پرداخت موفقیت را اعلام کرد، اما شناسه تراکنش دریافت نشد. نیاز به بررسی دارد."
        };
    }

    // زمان را بعد از پاسخ موفق درگاه ثبت می‌کنیم.
    var paidAt = DateTime.UtcNow;

    var finalizationResult = await repository.CompletePaymentAsync(
        payment.Id,
        order.Id,
        authority,
        verifyResult.TransactionId,
        paidAt);

    switch (finalizationResult)
    {
        case PaymentFinalizationResult.Completed:

            verifyResult.OrderConfirmed = true;
            return verifyResult;

        case PaymentFinalizationResult.LatePaymentRecorded:

            verifyResult.OrderConfirmed = false;
            verifyResult.ErrorMessage =
                "پرداخت موفق بوده، اما سفارش قبلاً لغو و موجودی آن آزاد شده است. پرداخت نیازمند بررسی مالی است.";

            return verifyResult;

        default:

            verifyResult.OrderConfirmed = false;
            verifyResult.ErrorMessage =
                "پرداخت در درگاه موفق بوده، اما ثبت نهایی آن در فروشگاه انجام نشد. نیاز به بررسی و تطبیق پرداخت دارد.";

            return verifyResult;
    }
}

    private static PaymentGatewayResult PaymentError(string message)
    {
        return new PaymentGatewayResult
        {
            Success = false,
            ErrorMessage = message
        };
    }

    private static PaymentGatewayVerifyResult VerifyError(string message)
    {
        return new PaymentGatewayVerifyResult
        {
            Success = false,
            ErrorMessage = message
        };
    }
}