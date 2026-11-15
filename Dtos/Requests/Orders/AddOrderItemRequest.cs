using System.ComponentModel.DataAnnotations;

namespace OnlineShop.Dtos.Requests;

public class AddOrderItemRequest
{
    [Range(1, int.MaxValue)] public required int ProductId { get; set; }
    [Range(1, int.MaxValue)]public required int Quantity { get; set; }
}