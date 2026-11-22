using Microsoft.Extensions.FileProviders;
using OnlineShop.Installer;
using OnlineShop.Setting;

var builder = WebApplication.CreateBuilder(args);
builder.Services.InstallServicesInAssembly(builder.Configuration);
builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();
// Configure the HTTP request pipeline.
app.MapOpenApi();
app.UseSwagger();
app.UseSwaggerUI();

app.UseStaticFiles();

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "Assets")),
    RequestPath = "/Assets"
});
//app.UseStaticFiles(new StaticFileOptions{FileProvider=new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(),"Assets")),RequestPath="/Assets"});

app.UseHttpsRedirection();

app.UseCors("Frontend");

app.UseMiddleware<JwtMiddleware>();

app.UseAuthorization();

app.MapControllers();

app.Run();
