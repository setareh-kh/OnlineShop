namespace OnlineShop.Dtos.Objects;

public class ZarinpalSettings
{
    public string MerchantId { get; set; } = string.Empty;

    public bool IsSandbox { get; set; } = true;

    public string SandboxBaseUrl { get; set; } =
        "https://sandbox.zarinpal.com";

    public string ProductionBaseUrl { get; set; } =
        "https://payment.zarinpal.com";
}