
namespace OnlineShop.Models.Enums;

public enum PaymentStatus
{
    Pending = 1,       // در انتظار پرداخت
    Processing = 2,    // در حال پردازش
    Successful = 3,    // پرداخت موفق
    Failed = 4,        // پرداخت ناموفق
    Cancelled = 5,     // پرداخت لغو شده
    Refunded = 6       // وجه بازپرداخت شده
}
