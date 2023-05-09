using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using VKPCShop.Data.EF;
using VKPCShop.Data.Entities;
using VKPCShop.WebApp.AutoMapper;
using VKPCShop.WebApp.Services.Brands;
using VKPCShop.WebApp.Services.Categories;
using VKPCShop.WebApp.Services.Images;
using VKPCShop.WebApp.Services.Orders;
using VKPCShop.WebApp.Services.Products;
using VKPCShop.WebApp.Services.Users;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<VKPCShopDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("VKPCShopDb"));
});


// Thêm RazorPage để xem được thay đổi
builder.Services.AddRazorPages()
    .AddRazorRuntimeCompilation();
// Thêm session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(1440);
});
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IBrandService, BrandService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IImageService, ImageService>();
builder.Services.AddScoped<IOrderService, OrderService>();

builder.Services.AddScoped<IProductService, ProductService>();


// Đăng ký AutoMapper
builder.Services.AddAutoMapper(typeof(AutoMapperProfile).Assembly);

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
    
    
}

app.UseSession();


app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession();  
app.UseRouting();


app.UseAuthentication();

app.UseAuthorization();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
      name: "areas",
      pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
    );
});

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
