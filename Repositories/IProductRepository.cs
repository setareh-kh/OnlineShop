using OnlineShop.Dtos.Requests;
using OnlineShop.Dtos.Responses;
using OnlineShop.Models;

namespace OnlineShop.Repositories
{
    public interface IProductRepository:IBaseRepository<Product>
    {
        Task<PaginateResponse<Product>> Filter(ProductFilterRequest filterDto);
        Task<List<Product>> GetAllWithCategoryAsync();
        Task<Product?> GetWithCategoryAsync(int id);
               
    }
}