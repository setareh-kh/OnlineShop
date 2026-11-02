using OnlineShop.Dtos.Requests;
using OnlineShop.Dtos.Responses;
using OnlineShop.Models;

namespace OnlineShop.Repositories
{
    public interface IOrderRepository:IBaseRepository<Order>
    {
        Task<PaginateResponse<Order>> Filter(OrderFilterRequest filterDto);
    }
}