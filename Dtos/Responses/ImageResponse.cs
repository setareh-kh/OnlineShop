namespace OnlineShop.Dtos.Responses;
public class ImageResponse
{
    public int Id { get; set; }
    public string Url { get; set; }=null!;
    public int Priority { get; set; }
}