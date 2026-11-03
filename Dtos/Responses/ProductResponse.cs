namespace OnlineShop.Dtos.Responses;

public class ProductResponse
{
    public required int Id { get; set; }
    public required string Name { get; set; }
    public required decimal Price { get; set; }
    public required int Quantity { get; set; }


}