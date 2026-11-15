namespace OnlineShop.Dtos.Responses;

public class ProductResponse
{
    public required int Id { get; set; }
    public required string Name { get; set; }
    public required decimal Price { get; set; }
    public required int Quantity { get; set; }
    public required int Discount { get; set; }
    public required bool Available { get; set; }
    public int CategoryId { get; set; }
    public CategoryResponse? Category { get; set; }
    public List<ImageResponse> Images { get; set; } = new();
    
    public decimal FinalPrice => Price * (1m - (Discount / 100m)); // =! Price * (100 - Discount) / 100m;

}