using OnlineShop.Dtos.Requests;
using OnlineShop.Dtos.Responses;

namespace OnlineShop.Services;

public interface IOrderService
{
    Task<PaginateResponse<OrderResponse>> Filter(OrderFilterRequest orderFilter);
    Task<OrderResponse?> CreateAsync(AddOrderRequest addOrder);

}