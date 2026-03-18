using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using WebBanHang.Data;
using WebBanHang.Data.Seeders;
using Microsoft.AspNetCore.Authentication.Cookies;
using WebBanHang.Helpers;
using WebBanHang.Middleware;
using WebBanHang.Repositories;
using WebBanHang.Repositories.SqlServer;
using System.Text.Json;
using System.Text.Json.Serialization;
using WebBanHang.Services.Interfaces;
using WebBanHang.Services.Implementations;
using WebBanHang.Services.Seller.Implementations;
using WebBanHang.Helpers.Product;
using WebBanHang.Services.Seller.Interfaces;
using WebBanHang.Helpers.Hubs;
using WebBanHang.Services.Global.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSignalR();


builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServerConnection")));

builder.Services.AddCors(opt =>
{
    opt.AddPolicy("react", p => p
    .WithOrigins("http://localhost:4200")
    .AllowAnyHeader()
    .AllowAnyMethod()
    .AllowCredentials());
});

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "WebBanHangAuthCookie";
        options.LoginPath = "/Login/SignIn";
        options.LogoutPath = "/Login/Logout";
        options.AccessDeniedPath = "/Login/AccessDenied";
        options.SlidingExpiration = true;
        options.ExpireTimeSpan = TimeSpan.FromHours(5);
        options.Cookie.HttpOnly = true;

        options.Cookie.SameSite = SameSiteMode.Lax;
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
        options.Events = new CookieAuthenticationEvents
        {
            OnValidatePrincipal = async context =>
            {
                Console.WriteLine($"🔐 Cookie validated at {DateTime.Now}");
            }
        };

    });

builder.Services.AddScoped(typeof(IRepository<>), typeof(SqlServerRepository<>));
//Repository
builder.Services.AddScoped<ICartRepository, SqlServerCartRepository>();
builder.Services.AddScoped<IOderRepository, SqlServerOderRepository>();
builder.Services.AddScoped<IProductRepository, SqlServerProductRepository>();
builder.Services.AddScoped<ISellerRepository, SqlServerSellerRepository>();
builder.Services.AddScoped<ISellerApplicationRepository, SqlServerSellerApplicationRepository>();
builder.Services.AddScoped<IUserRepository, SqlServerUserRepository>();
builder.Services.AddScoped<IRoleRepository, SqlServerRoleRepository>();
builder.Services.AddScoped<ICategoryRepository, SqlServerCategoryRepository>();
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();



//Services - Common
builder.Services.AddScoped<INotificationService, NotificationService>();

//Services - Admin
builder.Services.AddScoped<ISellerService, SellerService>();
builder.Services.AddScoped<ISellerApplicationService, SellerApplicationService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IAdminService, AdminService>();

//Services - Seller
builder.Services.AddScoped<IProductSellerService, ProductSellerService>();
builder.Services.AddScoped<ISellerInventoryService, SellerInventoryService>();
builder.Services.AddScoped<ISellerOrderService, SellerOrderService>();
builder.Services.AddScoped<ISellerRevenueService, SellerRevenueService>();
builder.Services.AddScoped<ISellerDashboardService, SellerDashboardService>();
builder.Services.AddScoped<ISellerSettingService, SellerSettingService>();

//Helper
builder.Services.AddScoped<FileHelper>();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });

builder.Services.AddAuthorization();
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await RoleSeeder.SeedAsync(db);
    await UserSeeder.SeedAsync(db);
}
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
// Configure the HTTP request pipeline.
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
//app.UseStatusCodePagesWithReExecute("/Login/AccessDenied", "?code={0}");
app.UseRouting();

app.UseCors("react");

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<PermissionMiddleware>();
app.UseMiddleware<AutoPermissionMiddleware>();

app.MapStaticAssets();

app.MapControllers();

app.MapHub<GlobalHub>("/globalhub");

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
