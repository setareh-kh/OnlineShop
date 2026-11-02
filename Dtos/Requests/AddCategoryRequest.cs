using System.ComponentModel.DataAnnotations;

namespace OnlineShop.Dtos.Requests;

public class AddCategoryRequest
{
    [Required]
    [MaxLength(250)]
    public required string Name { get; set; }
    [Required, MaxLength(500)]
    public string? Description { get; set; }
}