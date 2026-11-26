
using System.ComponentModel.DataAnnotations;

namespace OnlineShop.Models;

public class OrderItem : ISqlEntity
{
    public int Id { get; set; }

    // ارتباط با سفارش
    [Required]
    public int OrderId { get; set; }

    public Order Order { get; set; } = null!;

    // ارتباط اختیاری با محصول اصلی
    // اطلاعات Snapshot حتی پس از حذف محصول باقی می‌ماند.
    public int? ProductId { get; set; }

    public Product? Product { get; set; }

    // اطلاعات محصول در زمان خرید
    [Required]
    [MaxLength(200)]
    public string ProductName { get; set; } = null!;

    [MaxLength(1000)]
    public string? ImageUrl { get; set; }

    // قیمت و تخفیف در زمان خرید
    public decimal UnitPrice { get; set; }

    public int DiscountPercent { get; set; }

    public decimal FinalUnitPrice { get; set; }

    // تعداد خریداری شده
    public int Quantity { get; set; }

    // مجموع تخفیف این ردیف
    public decimal DiscountAmount { get; set; }

    // قیمت نهایی این ردیف
    public decimal TotalPrice { get; set; }

    public DateTime CreateAt { get; set; } = DateTime.UtcNow;
}
