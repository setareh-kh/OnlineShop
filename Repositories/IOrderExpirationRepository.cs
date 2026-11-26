
namespace OnlineShop.Repositories;

public interface IOrderExpirationRepository
{
    Task<int> ExpirePendingOrdersAsync();
}
