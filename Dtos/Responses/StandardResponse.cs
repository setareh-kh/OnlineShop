namespace OnlineShop.Dtos.Responses;

public class StandardResponse //fore Baseservice
{
    public bool Success { get; set; } = true;
    public string Message { get; set; } = "";
    public object? Object { get; set; }
}