using AutoMapper;
using OnlineShop.Dtos.Requests;
using OnlineShop.Dtos.Responses;
using OnlineShop.Models;
using OnlineShop.Repositories;

namespace OnlineShop.Services.Services;

public class CartService(
    ICartRepository cartRepository,
    IProductService productService,
    IImageService imageService, IMapper mapper) : ICartService
{
    public async Task<CartResponse> GetCartAsync(int userId)
    {
        var cart = await cartRepository.GetCartWithItemsAsync(userId);
        if (cart == null) return new CartResponse();
        var productIds = cart.Items.Select(x => x.ProductId).Distinct().ToList(); //List<int> productIds = cart.Items.Select(x => x.ProductId).ToList();
        // دریافت تصویر اصلی تمام محصولات سبد خرید
        var images = await imageService.GetMainProductImagesAsync(productIds);
        // تبدیل تصاویر به Dictionary بر اساس ProductId
        var imageDictionary = images.ToDictionary(x => x.ItemId);

        var response = new CartResponse
        {
            Id = cart.Id,

            Items = cart.Items.Select(item =>
            {
                imageDictionary.TryGetValue(item.ProductId, out var image);

                return new CartItemResponse
                {
                    Id = item.Id,
                    ProductId = item.ProductId,
                    Name = item.Product.Name,
                    UnitPrice = item.Product.Price,
                    Discount = item.Product.Discount,
                    Quantity = item.Quantity,
                    Image = image == null ? null : mapper.Map<ImageResponse>(image),
                    FinalPrice = item.Product.Price * (100 - item.Product.Discount) / 100,
                    Stock = item.Product.Quantity,
                    Available = item.Product.Available,
                    HasEnoughStock = item.Product.Active && item.Product.Available && item.Quantity <= item.Product.Quantity
                };
            }).ToList()
        };
        response.TotalPrice = response.Items.Sum(item => item.FinalPrice * item.Quantity);
        return response;

    }

    public async Task<bool> AddItemAsync(int userId, AddCartItemRequest request)
    {
        // 1. بررسی تعداد درخواستی
        if (request.Quantity <= 0)
            return false;

        // 2. بررسی محصول
        var product = await productService.GetAsync(request.ProductId);

        if (product == null ||
            !product.Active ||
            !product.Available ||
            product.Quantity < request.Quantity)
        {
            return false;
        }

        // 3. دریافت سبد خرید کاربر
        var cart = await cartRepository.GetCartWithItemsAsync(userId);

        // 4. اگر سبد خرید وجود نداشت، ایجاد آن
        if (cart == null)
        {
            cart = new Cart
            {
                UserId = userId,
                CreateAt = DateTime.UtcNow
            };

            await cartRepository.Insert(cart);
        }

        // 5. بررسی وجود محصول در سبد خرید
        var cartItem = cart.Items.FirstOrDefault(
            x => x.ProductId == request.ProductId);

        if (cartItem != null)
        {
            var newQuantity = cartItem.Quantity + request.Quantity;

            if (newQuantity > product.Quantity)
                return false;

            cartItem.Quantity = newQuantity;
            cartItem.UpdateDate = DateTime.UtcNow;
        }
        else
        {
            cart.Items.Add(new CartItem
            {
                Cart = cart, // در صورت وجود Navigation Property با این نام
                CartId = cart.Id,
                ProductId = product.Id,
                Quantity = request.Quantity,
                CreateAt = DateTime.UtcNow
            });
        }

        cart.UpdateDate = DateTime.UtcNow;

        // 6. ذخیره تغییرات
        return await cartRepository.SaveChangesAsync();
    }
    public async Task<bool> UpdateItemAsync(int userId, int cartItemId, UpdateCartItemRequest request)
    {
        if (request.Quantity <= 0)
            return false;

        // دریافت سبد خرید متعلق به کاربر
        var cart = await cartRepository.GetCartWithItemsAsync(userId);

        if (cart == null)
            return false;

        // پیدا کردن محصول فقط در سبد خرید همین کاربر
        var cartItem = cart.Items.FirstOrDefault(x => x.Id == cartItemId);

        if (cartItem == null)
            return false;

        var product = cartItem.Product;

        // بررسی وضعیت محصول و موجودی
        if (!product.Active ||
            !product.Available ||
            request.Quantity > product.Quantity)
        {
            return false;
        }

        // تغییر تعداد
        cartItem.Quantity = request.Quantity;
        cartItem.UpdateDate = DateTime.UtcNow;
        cart.UpdateDate = DateTime.UtcNow;

        return await cartRepository.SaveChangesAsync();
    }
    public async Task<bool> RemoveItemAsync(int userId, int cartItemId)
    {
        // دریافت سبد خرید کاربر
        var cart = await cartRepository.GetCartWithItemsAsync(userId);

        if (cart == null) return false;

        // جست‌وجو فقط در سبد خرید همین کاربر
        var cartItem = cart.Items.FirstOrDefault(x => x.Id == cartItemId);

        if (cartItem == null) return false;

        // حذف محصول از سبد
        cart.Items.Remove(cartItem);

        cart.UpdateDate = DateTime.UtcNow;

        return await cartRepository.SaveChangesAsync();
    }
}
