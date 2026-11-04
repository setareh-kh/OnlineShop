using OnlineShop.Models;

namespace OnlineShop.Dtos.Responses;

public class AdminUserResponse
{
    public required int Id { get; set; }
    public required string Name { get; set; }
    public required string Mobile { get; set; }
    public required UserType Type { get; set; }
    public bool? Active { get; set; }
    public required DateTime CreateAt { get; set; }
}