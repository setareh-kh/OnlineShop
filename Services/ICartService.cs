
using OnlineShop.Dtos.Requests;
using OnlineShop.Dtos.Responses;
using OnlineShop.Models;

namespace OnlineShop.Services;

public interface ICartService
{
    Task<CartResponse> GetCartAsync(int userId);
    Task<bool> AddItemAsync(int userId, AddCartItemRequest request);

}
