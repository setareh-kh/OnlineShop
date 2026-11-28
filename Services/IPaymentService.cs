using OnlineShop.Dtos.Objects;
using OnlineShop.Models;

namespace OnlineShop.Services;

public interface IPaymentService : IBaseService<Payment>
{
    Task<PaymentGatewayResult> StartPaymentAsync(
        int orderId,
        int userId,
        string callbackUrl);

    Task<PaymentGatewayVerifyResult> VerifyPaymentAsync(
        string authority,
        string status);
}