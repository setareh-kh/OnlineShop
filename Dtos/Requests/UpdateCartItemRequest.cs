using System.ComponentModel.DataAnnotations;

namespace OnlineShop.Dtos.Requests;

public class UpdateCartItemRequest
{
    [Required]
    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }
}