using OnlineShop.Dtos.Requests;
using OnlineShop.Dtos.Responses;
using OnlineShop.Models;

namespace OnlineShop.Repositories.Repositories
{
    public class CategoryRepository(SqlContext context) : BaseRepository<Category>(context), ICategoryRepository
    {
        public async Task<PaginateResponse<Category>> Filter(BaseFilterRequest filter)
        {
            var query = SqlContext.Categories.AsQueryable();
            // Search
            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                var search = filter.Search.Trim();
                query = query.Where(x => x.Name.Contains(search) || x.Description.Contains(search));
            }
            return await Paginate(filter, query);
        }
    }
}