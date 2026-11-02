
using OnlineShop.Dtos.Requests;
using OnlineShop.Dtos.Responses;
using OnlineShop.Models;

namespace OnlineShop.Services;

public interface ICategoryService:IBaseService<Category>
{
    Task<StandardResponse> GetAlldAsync();
    Task<StandardResponse> GetByIdAsync(int id);
    Task<StandardResponse> AddAsync(AddCategoryRequest addDto);
    Task<bool> UpdateAsync(int id, UpdateCategoryRequest updateCategory);
    Task<bool> DeleteAsync(int id);
}