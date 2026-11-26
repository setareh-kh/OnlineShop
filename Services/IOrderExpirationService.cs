
namespace OnlineShop.Services;

public interface IOrderExpirationService
{
    Task ExpirePendingOrdersAsync();
}
