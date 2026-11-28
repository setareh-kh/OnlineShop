using OnlineShop.Dtos.Objects;

namespace OnlineShop.Services;

public interface IPaymentGateway
{
    Task<PaymentGatewayResult> CreatePaymentAsync(
        PaymentGatewayRequest request,
        CancellationToken cancellationToken = default);

    Task<PaymentGatewayVerifyResult> VerifyPaymentAsync(
        PaymentGatewayVerifyRequest request,
        CancellationToken cancellationToken = default);
    string GetPaymentUrl(string authority);
}