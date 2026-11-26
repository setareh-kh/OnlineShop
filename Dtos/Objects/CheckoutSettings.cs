namespace OnlineShop.Dtos.Objects;

public class CheckoutSettings
{
    public decimal PostShippingCost { get; set; }
    public int OnlinePaymentReservationMinutes { get; set; } = 15;
}