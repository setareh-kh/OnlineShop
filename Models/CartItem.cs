
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace OnlineShop.Models;

[Index(nameof(CartId), nameof(ProductId), IsUnique = true)]
public class CartItem : ISqlEntity
{
    public int Id { get; set; }

    [Required]
    public required int CartId { get; set; }

    public Cart Cart { get; set; } = null!;

    [Required]
    public required int ProductId { get; set; }

    public Product Product { get; set; } = null!;

    [Required]
    public required int Quantity { get; set; }

    public DateTime CreateAt { get; set; } = DateTime.Now;

    public DateTime? UpdateDate { get; set; }
}
