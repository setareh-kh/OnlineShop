namespace OnlineShop.Dtos.Requests;

public class ProductFilterRequest : BaseFilterRequest
{
    public string? Name { get; set; }

    public int? MinPrice { get; set; }

    public int? MaxPrice { get; set; }

    public int? MinQuantity { get; set; }

    public int? MaxQuantity { get; set; }

    public int? MinDiscount { get; set; }

    public int? MaxDiscount { get; set; }

    public int? CategoryId { get; set; }

    public bool? Available { get; set; }
}
