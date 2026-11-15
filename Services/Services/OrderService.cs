using AutoMapper;
using OnlineShop.Dtos.Requests;
using OnlineShop.Dtos.Responses;
using OnlineShop.Models;
using OnlineShop.Repositories;

namespace OnlineShop.Services.Services
{
    public class OrderService(IOrderRepository orderRepository, 
                    IOrderDetailsRepository orderDetailsRepository,
                    IUserRepository userRepository,
                    IProductRepository productRepository,
                    IMapper mapper) : BaseService<Order>(orderRepository), IOrderService
    {
        public async Task<PaginateResponse<OrderResponse>> Filter(OrderFilterRequest orderFilter)
        {
            var res = await orderRepository.Filter(orderFilter);

            return new PaginateResponse<OrderResponse>()
            {
                Page = res.Page,
                Pager = res.Pager,
                Total = res.Total,
                Pages = res.Pages,
                Items = res.Items?.Select(mapper.Map<OrderResponse>).ToList()
            };
        }
        public async Task<OrderResponse?> CreateAsync(AddOrderRequest addOrder)
        {
            // 1. بررسی User
            var user = await userRepository.GetByIdAsync(addOrder.UserId);
            if (user == null)
                return null;
            // 2. ایجاد Order
            var order = new Order
            {
                UserId = addOrder.UserId,
                CreateAt = DateTime.UtcNow,
                TotalPrice = 0
            };
            await orderRepository.Insert(order);
            await orderRepository.SaveChangesAsync();
            decimal totalPrice = 0;
            // 3. پردازش محصولات
            foreach (var item in addOrder.Items)
            {
                var product = await productRepository.GetByIdAsync(item.ProductId);
                if (product == null)
                    return null;
                // 4. بررسی موجودی
                if (product.Quantity < item.Quantity)
                    return null;
                // محاسبه تخفیف
                decimal discountAmount = product.Price * product.Discount / 100m;
                // قیمت نهایی هر واحد
                decimal finalUnitPrice = product.Price - discountAmount;
                // قیمت کل این محصول
                decimal itemTotal = finalUnitPrice * item.Quantity;
                // اضافه کردن به مجموع سفارش
                totalPrice += itemTotal;
                // 5. ایجاد OrderDetails
                var orderDetail = new OrderDetails
                {
                    OrderId = order.Id,
                    Order = order,
                    ProductId = product.Id,
                    UnitPrice = product.Price,
                    Discount = product.Discount,
                    Quantity = item.Quantity,
                    CreateAt = DateTime.UtcNow
                };
                await orderDetailsRepository.Insert(orderDetail);
                // 6. کاهش موجودی
                product.Quantity -= item.Quantity;
                product.Available = product.Quantity > 0;
                product.UpdateDate = DateTime.UtcNow;
                productRepository.Update(product,product);
            }
            // قیمت نهایی کل سفارش
            order.TotalPrice = totalPrice;
            orderRepository.Update(order,order);
            // 7. ذخیره تمام تغییرات
            await orderRepository.SaveChangesAsync();
            // 8. ساخت Response
            return new OrderResponse
            {
                Id = order.Id,
                CreateAt = order.CreateAt,
                /*Items = addOrder.Items.Select(x => new OrderItemResponse
                {
                    ProductId = x.ProductId,
                    Quantity = x.Quantity
                }).ToList()*/
            };
        }

    }

}