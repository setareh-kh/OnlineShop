using OnlineShop.Models;

namespace OnlineShop.Repositories.Repositories
{
    public class OrderDetailsRepository(SqlContext context) : BaseRepository<OrderDetails>(context), IOrderDetailsRepository
    {
     
    }
}