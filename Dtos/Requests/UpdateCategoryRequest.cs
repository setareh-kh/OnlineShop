using System.ComponentModel.DataAnnotations;

namespace OnlineShop.Dtos.Requests;

public class UpdateCategoryRequest
{
    [MaxLength(250)]
    public string? Name { get; set; }
    [MaxLength(500)]
    public string? Description { get; set; }
}