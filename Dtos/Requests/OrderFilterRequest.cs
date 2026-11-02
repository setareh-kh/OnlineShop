namespace OnlineShop.Dtos.Requests;
public class OrderFilterRequest : BaseFilterRequest
{
    public int? UserId { get; set; }

    public decimal? MinTotalPrice { get; set; }
    public decimal? MaxTotalPrice { get; set; }

    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }

    public DateTime? FromUpdateDate { get; set; }
    public DateTime? ToUpdateDate { get; set; }
}