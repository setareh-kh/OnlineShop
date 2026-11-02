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

        var redisString = configuration.GetValue<string>("RedisConnectionStrings:Redis");
        var redis = ConnectionMultiplexer.Connect(redisString ?? "");
        services.AddHangfire(configuration =>
        {
            configuration
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseRedisStorage(redis,new RedisStorageOptions{Prefix = "{onlineshopping-hangfire}:", Db = 0});
        });


        services.AddHangfireServer(serverOptions =>
        {
            serverOptions.ServerName = "Hangfire.Redis server 1";
            serverOptions.WorkerCount = 2;
            serverOptions.Queues = new string[] {"default"}; // Define your queues
        });

        services.AddHangfireServer(serverOptions =>
        {
            serverOptions.ServerName = "Hangfire.Redis server 2";
            serverOptions.WorkerCount = 10;
            serverOptions.Queues = new string[] {"critical"}; // Define your queues
        });
    }
}