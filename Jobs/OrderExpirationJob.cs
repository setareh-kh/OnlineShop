
using OnlineShop.Services;

namespace OnlineShop.Jobs;

public class OrderExpirationJob(IOrderExpirationService orderExpirationService)
{

    public async Task ExecuteAsync()
    {
        await orderExpirationService.ExpirePendingOrdersAsync();
    }
}
