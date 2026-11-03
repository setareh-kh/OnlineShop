using Microsoft.Extensions.FileProviders;
using OnlineShop.Installer;
using OnlineShop.Setting;

var builder = WebApplication.CreateBuilder(args);
builder.Services.InstallServicesInAssembly(builder.Configuration);

var app = builder.Build();
// Configure the HTTP request pipeline.
app.MapOpenApi();
app.UseSwagger();
app.UseSwaggerUI();

//app.UseStaticFiles(new StaticFileOptions{FileProvider=new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(),"Assets")),RequestPath="/Assets"});

app.UseMiddleware<JwtMiddleware>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
