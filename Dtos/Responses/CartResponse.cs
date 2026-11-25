namespace OnlineShop.Dtos.Responses;

public class CartResponse
{
    public int Id { get; set; }

    public List<CartItemResponse> Items { get; set; } = new();

    public decimal TotalPrice { get; set; }
}