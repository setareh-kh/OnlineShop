using OnlineShop.Repositories.Repositories;
using OnlineShop.Repositories;
using OnlineShop.Dtos.Objects;
using OnlineShop.Services;
using OnlineShop.Services.Services;

namespace OnlineShop.Installer;

public class ServiceInstaller : IInstaller
{
    public void InstallServices(IConfiguration configuration, IServiceCollection services)
    {
        //install Irepositor and repository as services
        services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IImageRepository, ImageRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IOrderDetailsRepository, OrderDetailsRepository>();
        services.AddScoped<ITokenRepository, TokenRepository>();


        //install IService and Service as services
        services.AddScoped(typeof(IBaseService<>), typeof(BaseService<>));
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IImageService, ImageService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IAuthService, AuthService>();

        //For JWT
        var appSettings = configuration.GetSection("AppSettings").Get<AppSettings>();
        services.AddSingleton(appSettings!);
        services.AddHttpContextAccessor();


        //For RedisCache
        var redisConnectionString = configuration.GetValue<string>("RedisConnectionStrings:Redis");
        var perfixCach = configuration.GetValue<string>("RedisConnectionStrings:RedisPrefix");
        services.AddSingleton<IRedisService>(_ => new RedisService(redisConnectionString!, perfixCach!));
    }
}