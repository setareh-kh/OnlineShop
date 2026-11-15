namespace OnlineShop.Dtos.Responses;
public class OrderResponse
{
    public int Id { get; set; }
    public decimal TotalPrice { get; set; }
    public DateTime CreateAt { get; set; }
    public DateTime? UpdateDate { get; set; }
    public UserFilterResponse? User { get; set; }
    public List<OrderDetailsResponse> OrderDetails { get; set; } = new();
}
