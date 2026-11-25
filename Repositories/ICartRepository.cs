using OnlineShop.Models;
namespace OnlineShop.Repositories;

public interface ICartRepository : IBaseRepository<Cart>
{
    Task<Cart?> GetCartWithItemsAsync(int userId);

    Task<CartItem?> GetCartItemAsync(int cartId, int productId);
}
