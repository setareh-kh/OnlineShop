namespace OnlineShop.Installer;

public class OtherInstaller : IInstaller
{
    public void InstallServices(IConfiguration configuration, IServiceCollection services)
    {
 
        // Learn more about configuring Swagger(swashbuckle)/OpenAPI 
       services.AddEndpointsApiExplorer();
       services.AddSwaggerGen();
       
        //install Automapper services
       //services.AddAutoMapper(typeof(Program));
    }
}