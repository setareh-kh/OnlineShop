
using OnlineShop.Repositories;

namespace OnlineShop.Services.Services;

public class OrderExpirationService(IOrderExpirationRepository orderExpirationRepository): IOrderExpirationService
{
    public async Task ExpirePendingOrdersAsync()
    {
        var count = await orderExpirationRepository.ExpirePendingOrdersAsync();

        Console.WriteLine(
            $"Hangfire: {count} expired orders processed at {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}");
    }
}
