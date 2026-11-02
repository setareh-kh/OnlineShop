using Microsoft.EntityFrameworkCore;
using OnlineShop.Dtos.Requests;
using OnlineShop.Dtos.Responses;
using OnlineShop.Models;

namespace OnlineShop.Repositories.Repositories
{
    public class OrderRepository(SqlContext context): BaseRepository<Order>(context), IOrderRepository
    {
        public async Task<PaginateResponse<Order>> Filter(OrderFilterRequest filterDto)
        {
            var query = SqlContext.Orders.AsQueryable();

            // Search
            if (!string.IsNullOrWhiteSpace(filterDto.Search))
            {
                var search = filterDto.Search.Trim();
                query = query.Where(x => x.User.Name.Contains(search) ||
                    x.User.Mobile.Contains(search));
            }

            // User
            if (filterDto.UserId.HasValue)
            {
                query = query.Where(x => x.UserId == filterDto.UserId.Value);
            }

            // Min TotalPrice
            if (filterDto.MinTotalPrice.HasValue)
            {
                query = query.Where(x => x.TotalPrice >= filterDto.MinTotalPrice.Value);
            }

            // Max TotalPrice
            if (filterDto.MaxTotalPrice.HasValue)
            {
                query = query.Where(x => x.TotalPrice <= filterDto.MaxTotalPrice.Value);
            }

            // From CreateAt
            if (filterDto.FromDate.HasValue)
            {
                query = query.Where(x => x.CreateAt >= filterDto.FromDate.Value);
            }

            // To CreateAt
            if (filterDto.ToDate.HasValue)
            {
                var toDate = filterDto.ToDate.Value.Date.AddDays(1);
                query = query.Where(x => x.CreateAt < toDate);
            }

            // From UpdateDate
            if (filterDto.FromUpdateDate.HasValue)
            {
                query = query.Where(x =>
                    x.UpdateDate >= filterDto.FromUpdateDate.Value);
            }

            // To UpdateDate
            if (filterDto.ToUpdateDate.HasValue)
            {
                var toDate = filterDto.ToUpdateDate.Value.Date.AddDays(1);
                query = query.Where(x =>x.UpdateDate < toDate);
            }
            if (filterDto.Include)
            {
                query = query
                    .Include(x => x.User)
                    .Include(x => x.OrderDetails).ThenInclude(x => x.Product);
            }

            return await Paginate(filterDto, query);
        }
    }
}