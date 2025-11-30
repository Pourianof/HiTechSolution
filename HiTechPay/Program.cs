using HiTechPay;
using HiTechPay.Endpoints;
using HiTechPay.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddDependencies();

builder.Services.Configure<SignatureOptions>(
    builder.Configuration.GetSection("SignatureOptions"));

var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapApiEndpoints();

app.Run();