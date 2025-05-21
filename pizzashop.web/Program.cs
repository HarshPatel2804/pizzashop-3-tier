using Microsoft.EntityFrameworkCore;
using pizzashop.repository.Models;
using pizzashop.web;
using Microsoft.Extensions.DependencyInjection;
using pizzashop.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();
builder.Services.AddDbContext<PizzaShopContext>(options => 
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

DependencyInjection.RegisterServices(builder.Services, connectionString!);

// Add Lazy<T> resolution support to fix circular dependencies
builder.Services.AddTransient(typeof(Lazy<>), typeof(LazyResolution<>));

builder.Services.AddSession(options => {                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                 
    options.IdleTimeout = TimeSpan.FromDays(20);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.None;
});

builder.Services.AddSignalR();

var app = builder.Build();

app.UseStatusCodePagesWithReExecute("/Home/Error", "?statusCode={0}");



// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment()) {
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession();
app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapHub<OrderHub>("/orderHub");
app.MapHub<KOTHub>("/kotHub");
app.Run();

// This class helps resolve Lazy<T> services
public class LazyResolution<T> : Lazy<T> where T : class
{
    public LazyResolution(IServiceProvider serviceProvider) 
        : base(() => serviceProvider.GetRequiredService<T>())
    {
    }
}