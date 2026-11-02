using OnlineShop.Models;

namespace OnlineShop.Dtos.Requests;
public class UserFilterRequest : BaseFilterRequest
{
    public string? Name { get; set; }

    public string? Mobile { get; set; }

    public UserType? Type { get; set; }

    public DateTime? FromDate { get; set; }

    public DateTime? ToDate { get; set; }
}