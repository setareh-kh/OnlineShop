using Hangfire;
using Microsoft.Extensions.FileProviders;
using OnlineShop.Installer;
using OnlineShop.Jobs;
using OnlineShop.Setting;

var builder = WebApplication.CreateBuilder(args);
builder.Services.InstallServicesInAssembly(builder.Configuration);


var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapOpenApi();
app.UseSwagger();
app.UseSwaggerUI();

app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    DashboardTitle = "OnlineShop Dashboard",
});


app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "Assets")),
    RequestPath = "/Assets"
});


app.UseHttpsRedirection();

app.UseCors("Frontend"); // فعال‌کردن قانون CORS= چه وب‌سایت‌هایی اجازه دارند از مرورگر کاربر به ای پی ای پروژمون درخواست بفرستند

app.UseMiddleware<JwtMiddleware>();

app.UseAuthorization();

app.MapControllers();

var timezone = new RecurringJobOptions
{
    TimeZone = TimeZoneInfo.FindSystemTimeZoneById("Asia/Tehran")
};

/*RecurringJob.AddOrUpdate<EmailService>(
    "BirthdayEmail",
    "critical",
    x => x.SendVerificationEmailAsync("roshani.net","234567"),
    "0 13 * * *",
    timezone
);
RecurringJob.AddOrUpdate<IterativeJobs>("WriteHello","critical",j => j.WriteHello(),
    "* * * * *",
    timezone
);*/
RecurringJob.AddOrUpdate<OrderExpirationJob>(
    "expire-pending-orders",
    job => job.ExecuteAsync(),
    Cron.Minutely,
    timezone
);
app.Run();
