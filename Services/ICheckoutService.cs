using OnlineShop.Dtos.Requests;
using OnlineShop.Dtos.Responses;

namespace OnlineShop.Services;

public interface ICheckoutService
{
    Task<CheckoutPreviewResponse> PreviewAsync(int userId);
    Task<CheckoutResponse> CreateOrderAsync(int userId,CheckoutRequest request,string idempotencyKey);
}