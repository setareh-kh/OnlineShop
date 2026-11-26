using System.ComponentModel.DataAnnotations;
using OnlineShop.Models.Enums;

namespace OnlineShop.Dtos.Requests;

public class CheckoutRequest
{
    [Required, MaxLength(150)]
    public required string RecipientName { get; set; }

    [Required, MaxLength(20)]
    public required string RecipientMobile { get; set; }

    [Required, MaxLength(100)]
    public required string Province { get; set; }

    [Required, MaxLength(100)]
    public required string City { get; set; }

    [Required, MaxLength(500)]
    public required string Address { get; set; }

    [Required, MaxLength(20)]
    public required string PostalCode { get; set; }

    [MaxLength(1000)]
    public string? Description { get; set; }

    [Required]
    [EnumDataType(typeof(PaymentMethod))]
    public PaymentMethod PaymentMethod { get; set; }

    [Required]
    [EnumDataType(typeof(ShippingMethod))]
    public ShippingMethod ShippingMethod { get; set; }
}