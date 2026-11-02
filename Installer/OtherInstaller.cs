namespace OnlineShop.Installer;

public class OtherInstaller : IInstaller
{
    public void InstallServices(IConfiguration configuration, IServiceCollection services)
    {
        services.AddControllers();
        services.AddOpenApi();
        services.AddSwaggerGen();
        services.AddAutoMapper(cfg => { }, typeof(Program));
    }
}