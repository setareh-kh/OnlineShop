namespace OnlineShop.Dtos.Objects;
public class PaymentGatewayRequest
{
    public decimal AmountToman { get; set; }

    public string Description { get; set; } = null!;

    public string CallbackUrl { get; set; } = null!;

    public string? Mobile { get; set; }

    public string? Email { get; set; }
}