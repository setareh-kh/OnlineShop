namespace OnlineShop.Dtos.Objects;

public class PaymentGatewayResult
{
    public bool Success { get; set; }

    public string? Authority { get; set; }

    public string? PaymentUrl { get; set; }

    public int? ErrorCode { get; set; }

    public string? ErrorMessage { get; set; }
}