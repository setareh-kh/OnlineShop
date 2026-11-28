namespace OnlineShop.Dtos.Objects;

public class PaymentGatewayVerifyResult
{
    public bool Success { get; set; }

    public bool AlreadyVerified { get; set; }

    public bool OrderConfirmed { get; set; }

    public string? TransactionId { get; set; }

    public int? ErrorCode { get; set; }

    public string? ErrorMessage { get; set; }
}