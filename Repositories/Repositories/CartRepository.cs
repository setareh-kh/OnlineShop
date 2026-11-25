using Microsoft.EntityFrameworkCore;
using OnlineShop.Models;


namespace OnlineShop.Repositories.Repositories;

public class CartRepository(SqlContext context): BaseRepository<Cart>(context), ICartRepository
{
    //سبد خرید کاربر را همراه محصولات داخل آن دریافت می‌کند.
    public async Task<Cart?> GetCartWithItemsAsync(int userId) 
    {
        return await SqlContext.Carts
            .Include(c => c.Items)
                .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(c => c.UserId == userId);
    }

    //بررسی می‌کند محصول موردنظر قبلاً در سبد وجود دارد یا خیر.
    public async Task<CartItem?> GetCartItemAsync(int cartId, int productId)
    {
        return await SqlContext.CartItems
            .FirstOrDefaultAsync(x =>
                x.CartId == cartId &&
                x.ProductId == productId);
    }
}
