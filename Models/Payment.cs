
using System.ComponentModel.DataAnnotations;
using OnlineShop.Models.Enums;

namespace OnlineShop.Models;

public class Payment : ISqlEntity
{
    public int Id { get; set; }

    // ارتباط با سفارش
    [Required]
    public int OrderId { get; set; }

    public Order Order { get; set; } = null!;

    // مبلغ پرداخت
    public decimal Amount { get; set; }

    // روش پرداخت
    public PaymentMethod Method { get; set; }

    // وضعیت پرداخت
    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;

    // اطلاعات درگاه پرداخت
    [MaxLength(100)]
    public string? Gateway { get; set; }

    [MaxLength(200)]
    public string? Authority { get; set; }

    [MaxLength(200)]
    public string? TransactionId { get; set; }

    // دلیل ناموفق بودن پرداخت
    [MaxLength(500)]
    public string? FailureReason { get; set; }

    // تاریخ پرداخت موفق
    public DateTime? PaidAt { get; set; }

    // تاریخ‌ها
    public DateTime CreateAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdateDate { get; set; }
}
