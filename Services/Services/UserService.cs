using System.Security.Cryptography;
using OnlineShop.Dtos.Requests;
using OnlineShop.Dtos.Responses;
using OnlineShop.Models;
using OnlineShop.Repositories;

namespace OnlineShop.Services.Services;

public class UserService(IUserRepository userRepository, IRedisService redisService
            , IImageService imageService) : BaseService<User>(userRepository), IUserService
{
    
    public async Task<StandardResponse> GetAlldAsync()
    {

        if (redisService.KeyExists($"all_users"))
            return Success((await redisService.GetValueAsync<List<User>>($"all_users"))!);
        else
        {
            var users = await GetAll();
            await redisService.SetValueAsync($"all_users", users!, TimeSpan.FromMinutes(10));
            return Success(users!);
        }

    }
    public async Task<StandardResponse> GetByIdAsync(int id)
    {
        if (redisService.KeyExists($"user_{id}"))
            return Success((await redisService.GetValueAsync<User>($"user_{id}"))!);
        var user = await GetAsync(id);
        if (user == null) return NotFound();
        await redisService.SetValueAsync($"user_{id}", user!, TimeSpan.FromMinutes(10));
        return Success(user);
    }
    public async Task<bool> UpdateAsync(int id, UpdateUserRequest updateUser)
    {
        var user = await userRepository.GetByIdAsync(id);
        if (user == null) return false;
        if (!string.IsNullOrWhiteSpace(updateUser.Name)) user.Name = updateUser.Name;
        if (updateUser.Image?.Length > 0)
        {
            var img = await imageService.SaveAsync(updateUser.Image, "User", id);
        }
        await userRepository.SaveChangesAsync(); //Not use UpdateAsync of BaseRepo because It use from Setvalue EF
        await redisService.SetValueAsync($"user_{id}", user!, TimeSpan.FromMinutes(10));
        redisService.DeleteKey($"all_users");
        return true;
    }
    public async Task<bool> DeleteAsync(int id)
    {
        var user = await userRepository.GetByIdAsync(id);
        if (user == null) return false;
        if (redisService.KeyExists($"user_{id}"))
            redisService.DeleteKey($"user_{id}");
        redisService.DeleteKey($"all_users");
        userRepository.Delete(user);
        await userRepository.SaveChangesAsync();
        await imageService.DeleteAsync("User", id);
        return true;
    }
    public async Task<PaginateResponse<User>> Filter(UserFilterRequest filter)
    {
        var res = await userRepository.Filter(filter);
        return new PaginateResponse<User>()
        {
            Page = res.Page,
            Pager = res.Pager,
            Total = res.Total,
            Pages = res.Pages,
            Items = res.Items?.ToList()
        };
    }

    
}