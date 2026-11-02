using OnlineShop.Dtos.Requests;
using OnlineShop.Dtos.Responses;
using OnlineShop.Models;

namespace OnlineShop.Repositories
{
    public interface ICategoryRepository : IBaseRepository<Category>
    {
        Task<PaginateResponse<Category>> Filter(BaseFilterRequest filter);
    }
}