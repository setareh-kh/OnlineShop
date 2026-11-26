using OnlineShop.Dtos.Objects;
using OnlineShop.Dtos.Requests;

namespace OnlineShop.Repositories;

public interface ICheckoutRepository
{
    Task<CheckoutCreateResult> CreateOrderAsync(
        int userId,
        CheckoutRequest request,
        string idempotencyKey,
        decimal shippingCost,
        int reservationMinutes);
}