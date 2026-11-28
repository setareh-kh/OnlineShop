namespace OnlineShop.Dtos.Objects;

public class PaymentGatewayVerifyRequest
{
    public string Authority { get; set; } = null!;

    public decimal AmountToman { get; set; }
}