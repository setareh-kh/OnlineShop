
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace OnlineShop.Models;

[Index(nameof(UserId), IsUnique = true)]
public class Cart : ISqlEntity
{
    public int Id { get; set; }

    [Required]
    public required int UserId { get; set; }

    public User User { get; set; } = null!;

    public DateTime CreateAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdateDate { get; set; }

    public ICollection<CartItem> Items { get; set; } = new List<CartItem>();
}
