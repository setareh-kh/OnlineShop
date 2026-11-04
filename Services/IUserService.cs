using OnlineShop.Dtos.Requests;
using OnlineShop.Dtos.Responses;
using OnlineShop.Models;

namespace OnlineShop.Services;

public interface IUserService : IBaseService<User>
{
    Task<StandardResponse> GetAlldAsync();
    Task<StandardResponse> GetByIdAsync(int id);
    Task<bool> UpdateAsync(int id, UpdateUserRequest updateUser);
    Task<bool> DeleteAsync(int id);
    Task<PaginateResponse<User>> Filter(UserFilterRequest filter);
    Task<bool> ChangeActive(int id, bool newActive);

}