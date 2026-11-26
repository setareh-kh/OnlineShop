using OnlineShop.Models.Enums;

namespace OnlineShop.Dtos.Responses;

public class CheckoutResponse
{
    public int OrderId { get; set; }

    public string OrderNumber { get; set; } = null!;

    public OrderStatus Status { get; set; }

    public PaymentStatus PaymentStatus { get; set; }

    public PaymentMethod PaymentMethod { get; set; }

    public ShippingMethod ShippingMethod { get; set; }

    public decimal TotalPrice { get; set; }

    public DateTime CreateAt { get; set; }
}