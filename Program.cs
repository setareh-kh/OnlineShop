using OnlineShop.Installer;

var builder = WebApplication.CreateBuilder(args);
builder.Services.InstallServicesInAssembly(builder.Configuration);

var app = builder.Build();
// Configure the HTTP request pipeline.
app.MapOpenApi();
app.UseSwagger();
app.UseSwaggerUI();


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
