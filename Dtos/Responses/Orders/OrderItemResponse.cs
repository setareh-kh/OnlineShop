namespace OnlineShop.Dtos.Responses;
public class OrderItemResponse
{
    public int ProductId { get; set; }

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public int Discount { get; set; }
}