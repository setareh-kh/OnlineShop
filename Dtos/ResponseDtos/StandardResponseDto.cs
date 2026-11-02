namespace OnlineShop.Dtos.ResponseDtos;

public class StandardResponseDto //fore Baseservice
{
    public bool Success { get; set; } = true;
    public string Message { get; set; } = "";
    public dynamic? Object { get; set; }
}