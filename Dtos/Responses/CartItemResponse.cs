namespace OnlineShop.Dtos.Responses;

public class CartItemResponse
{
    public int Id { get; set; }

    public int ProductId { get; set; }

    public string Name { get; set; } = null!;

    public decimal UnitPrice { get; set; }

    public decimal Discount { get; set; }

    public int Quantity { get; set; }

    public decimal FinalPrice { get; set; }
    public ImageResponse? Image { get; set; }
    public int Stock { get; set; }
    public bool Available { get; set; }
    public bool HasEnoughStock { get; set; }

}