using OnlineShop.Models;

namespace OnlineShop.Repositories.Repositories
{
    public class CategoryRepository(SqlContext context): BaseRepository<Category>(context),ICategoryRepository
    {
    
    }
}