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
                    HasEnoughStock = item.Product.Available &&
                 item.Quantity <= item.Product.Quantity
                };
            }).ToList()
        };
        response.TotalPrice = response.Items.Sum(item => item.FinalPrice * item.Quantity);
        return response;

    }

    public async Task<bool> AddItemAsync(int userId, AddCartItemRequest request)
    {
        // 1. بررسی محصول
        var product = await productService.GetAsync(request.ProductId);

        if (product == null ||
            !product.Active ||
            !product.Available ||
            product.Quantity < 1)
        {
            return false;
        }

        // 2. دریافت سبد خرید کاربر
        var cart = await cartRepository.GetCartWithItemsAsync(userId);

        // 3. اگر سبد وجود نداشت، فقط به Context اضافه می‌کنیم
        if (cart == null)
        {
            cart = new Cart
            {
                UserId = userId,
                CreateAt = DateTime.UtcNow
            };

            await cartRepository.Insert(cart);
        }

        // 4. بررسی وجود محصول در سبد
        var cartItem = cart.Items.FirstOrDefault(
            x => x.ProductId == request.ProductId);

        if (cartItem != null)
        {
            // افزایش تعداد محصول موجود در سبد
            var newQuantity = cartItem.Quantity + request.Quantity;

            if (newQuantity > product.Quantity)
                return false;

            cartItem.Quantity = newQuantity;
            cartItem.UpdateDate = DateTime.UtcNow;
        }
        else
        {
            // افزودن محصول جدید
            cart.Items.Add(new CartItem
            {
                CartId = cart.Id,
                ProductId = product.Id,
                Quantity = request.Quantity,
                CreateAt = DateTime.UtcNow
            });
        }

        cart.UpdateDate = DateTime.UtcNow;

        // 5. فقط یک بار ذخیره تغییرات
        return await cartRepository.SaveChangesAsync();
    }
}
