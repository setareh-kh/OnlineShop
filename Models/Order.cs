
using System.ComponentModel.DataAnnotations;
using OnlineShop.Models.Enums;

namespace OnlineShop.Models;

public class Order : ISqlEntity
{
    public int Id { get; set; }

    // شماره سفارش قابل نمایش به مشتری
    [Required]
    [MaxLength(50)]
    public string OrderNumber { get; set; } = null!;

    // ارتباط با کاربر
    [Required]
    public int UserId { get; set; }

    public User User { get; set; } = null!;

    // وضعیت سفارش
    public OrderStatus Status { get; set; } = OrderStatus.PendingPayment;

    // وضعیت پرداخت سفارش
    public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;

    // روش انتخاب شده برای پرداخت
    public PaymentMethod PaymentMethod { get; set; }

    // روش ارسال
    public ShippingMethod ShippingMethod { get; set; } = ShippingMethod.Post;

    // مبالغ سفارش
    public decimal SubTotal { get; set; } //مبلغ کالاها قبل از تخفیف 

    public decimal DiscountTotal { get; set; } //مجموع تخفیف تمام کالاهاست

    public decimal ShippingCost { get; set; } //هزینه ارسال 

    public decimal TotalPrice { get; set; } //مبلغ نهایی= TotalPrice = SubTotal - DiscountTotal + ShippingCost

    // اطلاعات گیرنده
    [Required]
    [MaxLength(150)]
    public string RecipientName { get; set; } = null!;

    [Required]
    [MaxLength(20)]
    public string RecipientMobile { get; set; } = null!;

    // آدرس ارسال به صورت Snapshot
    [Required]
    [MaxLength(100)]
    public string Province { get; set; } = null!;

    [Required]
    [MaxLength(100)]
    public string City { get; set; } = null!;

    [Required]
    [MaxLength(500)]
    public string Address { get; set; } = null!;

    [Required]
    [MaxLength(20)]
    public string PostalCode { get; set; } = null!;

    [MaxLength(1000)]
    public string? Description { get; set; }

    // اطلاعات مرسوله
    [MaxLength(100)]
    public string? ShippingTrackingCode { get; set; }

    public DateTime? ShippedAt { get; set; }

    public DateTime? DeliveredAt { get; set; }

    // اطلاعات لغو سفارش
    [MaxLength(500)]
    public string? CancellationReason { get; set; }

    public DateTime? CancelledAt { get; set; }

    // کنترل رزرو موجودی
    public DateTime? InventoryReservedAt { get; set; }

    public DateTime? InventoryReservedUntil { get; set; }

    public DateTime? InventoryReleasedAt { get; set; }

    // تاریخ‌ها
    public DateTime CreateAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdateDate { get; set; }

    // اقلام سفارش
    public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();

    // پرداخت‌های سفارش
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
