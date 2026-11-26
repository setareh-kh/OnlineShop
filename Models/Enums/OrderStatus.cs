
namespace OnlineShop.Models.Enums;

public enum OrderStatus
{
    PendingPayment = 1, // در انتظار پرداخت
    Confirmed = 2,      // تأیید شده
    Processing = 3,     // در حال آماده‌سازی
    Shipped = 4,        // ارسال شده
    Delivered = 5,      // تحویل داده شده
    Cancelled = 6,      // لغو شده
    Refunded = 7        // بازپرداخت شده
}
