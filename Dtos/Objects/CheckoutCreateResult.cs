using OnlineShop.Models;

namespace OnlineShop.Dtos.Objects;

public class CheckoutCreateResult
{
    public bool Success { get; set; }

    public string? Message { get; set; }

    public Order? Order { get; set; }
}