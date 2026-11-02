using OnlineShop.Models;


namespace OnlineShop.Repositories.Repositories
{
    public class TokenRepository(SqlContext context) : BaseRepository<Token>(context), ITokenRepository 
    {

    }
}