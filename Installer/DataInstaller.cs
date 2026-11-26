using Hangfire;
using Hangfire.Redis.StackExchange;
using Microsoft.EntityFrameworkCore;
using OnlineShop.Installer;
using OnlineShop.Models;
using StackExchange.Redis;


namespace OnlineShop.Dtos.Objects;

public class DataInstaller : IInstaller
{
    public void InstallServices(IConfiguration configuration, IServiceCollection services)
    {
        //mysql connection Setting
        var myConnection = configuration.GetConnectionString("MySqlConnection") ?? throw new InvalidOperationException(
            "Connection string 'MySqlConnection' was not found.");
        services.AddDbContext<SqlContext>(options => options.UseMySQL(myConnection));

        var redisConnectionString =configuration.GetValue<string>("RedisConnectionStrings:Redis")?? throw new InvalidOperationException(
        "Redis connection string not found.");

        services.AddHangfire(configuration =>
        {
            configuration.UseRedisStorage(
                redisConnectionString,
                new RedisStorageOptions
                {
                    Prefix = "{OnlineShop:Hangfire}:"
                });
        });

        services.AddHangfireServer(options =>
        {
            options.WorkerCount = 2;
        });

    }
}