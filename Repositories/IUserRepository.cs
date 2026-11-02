using OnlineShop.Dtos.Requests;
using OnlineShop.Dtos.Responses;
using OnlineShop.Models;

namespace OnlineShop.Repositories
{
    public interface IUserRepository : IBaseRepository<User>
    {
        Task<PaginateResponse<User>> Filter(UserFilterRequest filter);

    }
}