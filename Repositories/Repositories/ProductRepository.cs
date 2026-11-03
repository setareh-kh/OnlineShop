using Microsoft.EntityFrameworkCore;
using OnlineShop.Dtos.Requests;
using OnlineShop.Dtos.Responses;
using OnlineShop.Models;
namespace OnlineShop.Repositories.Repositories
{
    public class ProductRepository(SqlContext context) : BaseRepository<Product>(context), IProductRepository
    {

        public async Task<PaginateResponse<Product>> Filter(ProductFilterRequest filterDto)
        {
            var query = SqlContext.Products.AsQueryable();
            // Search
            if (!string.IsNullOrWhiteSpace(filterDto.Search))
            {
                var search = filterDto.Search.Trim();
                query = query.Where(x => x.Name.Contains(search));
            }
             // Active
            if (filterDto.Active.HasValue)
            {
                query = query.Where(x => x.Active == filterDto.Active.Value);
            }

            // Name
            if (!string.IsNullOrWhiteSpace(filterDto.Name))
            {
                var name = filterDto.Name.Trim();
                query = query.Where(x => x.Name.Contains(name));
            }

            // Min Price
            if (filterDto.MinPrice.HasValue)
            {
                query = query.Where(x => x.Price >= filterDto.MinPrice.Value);
            }

            // Max Price
            if (filterDto.MaxPrice.HasValue)
            {
                query = query.Where(x => x.Price <= filterDto.MaxPrice.Value);
            }

            // Min Quantity
            if (filterDto.MinQuantity.HasValue)
            {
                query = query.Where(x => x.Quantity >= filterDto.MinQuantity.Value);
            }

            // Max Quantity
            if (filterDto.MaxQuantity.HasValue)
            {
                query = query.Where(x => x.Quantity <= filterDto.MaxQuantity.Value);
            }

            // Min Discount
            if (filterDto.MinDiscount.HasValue)
            {
                query = query.Where(x => x.Discount >= filterDto.MinDiscount.Value);
            }

            // Max Discount
            if (filterDto.MaxDiscount.HasValue)
            {
                query = query.Where(x => x.Discount <= filterDto.MaxDiscount.Value);
            }
            // Available
            if (filterDto.Available.HasValue)
            {
                query = query.Where(x => x.Available == filterDto.Available.Value);
            }

            // Category
            if (filterDto.CategoryId.HasValue)
            {
                query = query.Where(x => x.CategoryId == filterDto.CategoryId.Value);
            }

            // Include Category
            if (filterDto.Include)
            {
                query = query.Include(x => x.Category);
            }

            return await Paginate(filterDto, query);
        }

    }
}