using AutoMapper;
using OnlineShop.Dtos.Requests;
using OnlineShop.Dtos.Responses;
using OnlineShop.Models;
using OnlineShop.Repositories;

namespace OnlineShop.Services.Services;

public class CategoryService(ICategoryRepository categoryRepository, IRedisService redisService, 
            IMapper mapper) : BaseService<Category>(categoryRepository), ICategoryService
{

    public async Task<StandardResponse> GetAlldAsync()
    {

        if (redisService.KeyExists($"all_categories"))
            return Success((await redisService.GetValueAsync<List<Category>>($"all_categories"))!);
        else
        {
            var categories = await GetAll();
            await redisService.SetValueAsync($"all_categories", categories!, TimeSpan.FromMinutes(10));
            return Success(categories!);
        }

    }
    public async Task<StandardResponse> GetByIdAsync(int id)
    {
        if (redisService.KeyExists($"category_{id}"))
            return Success((await redisService.GetValueAsync<Category>($"category_{id}"))!);
        var category = await GetAsync(id);
        if (category == null) return NotFound();
        await redisService.SetValueAsync($"category_{id}", category!, TimeSpan.FromMinutes(10));
        return Success(category);
    }

    public async Task<StandardResponse> AddAsync(AddCategoryRequest addDto)
    {
        var category = mapper.Map<Category>(addDto);
        var id = await categoryRepository.InsertAsync(category);
        await redisService.SetValueAsync($"category_{id}", category, TimeSpan.FromMinutes(10));
        redisService.DeleteKey($"all_categories");
        return Success(category);
    }
    public async Task<bool> UpdateAsync(int id, UpdateCategoryRequest updateCategory)
    {
        if (redisService.KeyExists($"category_{id}"))
            redisService.DeleteKey($"category_{id}");
        var category = await categoryRepository.GetByIdAsync(id);
        if (category == null) return false;
        mapper.Map(updateCategory, category);
        await categoryRepository.SaveChangesAsync(); //Not use UpdateASync of BaseRepo because It use from Setvalue EF
        await redisService.SetValueAsync($"category_{id}", category!, TimeSpan.FromMinutes(10));
        redisService.DeleteKey($"all_categories");
        return true;
    }
    public async Task<bool> DeleteAsync(int id)
    {
        var category = await categoryRepository.GetByIdAsync(id);
        if (category == null) return false;
        if (redisService.KeyExists($"category_{id}"))
            redisService.DeleteKey($"category_{id}");
        redisService.DeleteKey($"all_categories");
        categoryRepository.Delete(category);
        await categoryRepository.SaveChangesAsync();
        return true;
    }
    public async Task<PaginateResponse<Category>> Filter(BaseFilterRequest filter)
    {
        var res = await categoryRepository.Filter(filter);
        return new PaginateResponse<Category>()
        {
            Page = res.Page,
            Pager = res.Pager,
            Total = res.Total,
            Pages = res.Pages,
            Items = res.Items?.ToList()
        };
    }


}