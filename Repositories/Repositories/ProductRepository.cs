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
            var query = SqlContext.Products.AsNoTracking().Include(p => p.Category).AsQueryable();

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

            // MinFinalPrice
            if (filterDto.MinFinalPrice.HasValue)
            {
                query = query.Where(p => p.Price * (1m - (p.Discount / 100m)) >= filterDto.MinFinalPrice.Value);
                //query = query.Where(p =>p.Price * (100 - p.Discount) / 100m >= filterDto.MinFinalPrice.Value);

            }

            // MaxFinalPrice
            if (filterDto.MaxFinalPrice.HasValue)
            {
                query = query.Where(p => p.Price * (1m - (p.Discount / 100m)) <= filterDto.MaxFinalPrice.Value);
                //query = query.Where(p =>p.Price * (100 - p.Discount) / 100m <= filterDto.MaxFinalPrice.Value);
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
            /* if (filterDto.Include)
             {
                 query = query.Include(x => x.Category);
             }*/

            return await Paginate(filterDto, query);
        }
        public async Task<List<Product>> GetAllWithCategoryAsync()
        {
            return await SqlContext.Products
                .Include(p => p.Category)
                .ToListAsync();
        }
        public async Task<Product?> GetWithCategoryAsync(int id)
        {
            return await SqlContext.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);
        }


    }
}