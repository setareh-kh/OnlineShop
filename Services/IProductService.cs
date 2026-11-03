
using OnlineShop.Dtos.Requests;
using OnlineShop.Dtos.Responses;
using OnlineShop.Models;

namespace OnlineShop.Services;

public interface IProductService:IBaseService<Product>
{
    Task<Product?> AddAsync(AddProductRequest dto);
    Task<bool> DeleteAsync(int id);
    Task<bool> UpdateAsync(int id, UpdateProductRequest updateProduct);
    Task<PaginateResponse<ProductResponse>> Filter(ProductFilterRequest filterDto);
}