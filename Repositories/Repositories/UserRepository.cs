
using Microsoft.EntityFrameworkCore;
using OnlineShop.Dtos.Requests;
using OnlineShop.Dtos.Responses;
using OnlineShop.Models;

namespace OnlineShop.Repositories.Repositories
{
    public class UserRepository(SqlContext context) : BaseRepository<User>(context), IUserRepository
    {
        public async Task<PaginateResponse<User>> Filter(UserFilterRequest filter)
        {
            var query = SqlContext.Users.AsQueryable();
            // Search
            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                var search = filter.Search.Trim();
                query = query.Where(x => x.Name.Contains(search) || x.Mobile.Contains(search));
            }
            // Active
            if (filter.Active != null)
            {
                query = query.Where(x => x.Active == filter.Active);
            }

            // Name
            if (!string.IsNullOrWhiteSpace(filter.Name))
            {
                var name = filter.Name.Trim();
                query = query.Where(x => x.Name.Contains(name));
            }

            // Mobile
            if (!string.IsNullOrWhiteSpace(filter.Mobile))
            {
                var mobile = filter.Mobile.Trim();
                query = query.Where(x => x.Mobile.Contains(mobile));
            }

            // Type
            if (filter.Type.HasValue)
            {
                query = query.Where(x => x.Type == filter.Type.Value);
            }

            // FromDate
            if (filter.FromDate.HasValue)
            {
                query = query.Where(x => x.CreateAt >= filter.FromDate.Value);
            }

            // ToDate
            if (filter.ToDate.HasValue)
            {
                query = query.Where(x => x.CreateAt <= filter.ToDate.Value);
            }

            return await Paginate(filter, query);
        }


    }
}