using System.ComponentModel.DataAnnotations;
namespace OnlineShop.Dtos.Requests;
public class AddOrderRequest
{
    [Range(1, int.MaxValue)] public required int UserId { get; set; }
    [Required][MinLength(1)] public List<AddOrderItemRequest> Items { get; set; } = new();
}