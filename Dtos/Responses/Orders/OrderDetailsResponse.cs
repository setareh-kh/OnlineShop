namespace OnlineShop.Dtos.Responses;
public class OrderDetailsResponse
{
    public int Id { get; set; }
    public ProductResponse? Product { get; set; }
    public decimal UnitPrice { get; set; }
    public int Discount { get; set; }
    public int Quantity { get; set; }
    public DateTime CreateAt { get; set; }
    public DateTime? UpdateDate { get; set; }
}

