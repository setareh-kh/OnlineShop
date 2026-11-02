using OnlineShop.Repositories.Repositories;
using OnlineShop.Repositories;
using OnlineShop.Dtos.Objects;

namespace OnlineShop.Installer;

public class ServiceInstaller : IInstaller
{
    public void InstallServices(IConfiguration configuration, IServiceCollection services)
    {
        //install Irepositor and repository as services
        services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
        

        //install IService and Service as services
       

        var appSettings = configuration.GetSection("AppSettings").Get<AppSettings>();
        services.AddSingleton(appSettings!);
        services.AddHttpContextAccessor();

        var redisConnectionString= configuration.GetValue<string>("RedisConnectionStrings:Redis");
        var perfixCach= configuration.GetValue<string>("RedisConnectionStrings:RedisPrefix");
        //services.AddSingleton<IRedisService>(_ =>new RedisService(redisConnectionString!,perfixCach!));
    }
}