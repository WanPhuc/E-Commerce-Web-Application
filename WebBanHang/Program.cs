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
using WebBanHang;
using Microsoft.OpenApi;
using System.Security.Cryptography.Xml;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "NHap token o day"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            }, new string[] { }
        }
    });
});

builder.Services.AddSignalR();


builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServerConnection")));

builder.Services.AddCors(opt =>
{
    opt.AddPolicy("angular", p => p
    .WithOrigins("http://localhost:4200")
    .AllowAnyHeader()
    .AllowAnyMethod()
    .AllowCredentials());
});

//builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
//    .AddCookie(options =>
//    {
//        options.Cookie.Name = "WebBanHangAuthCookie";
//        options.LoginPath = "/Login/SignIn";
//        options.LogoutPath = "/Login/Logout";
//        options.AccessDeniedPath = "/Login/AccessDenied";
//        options.SlidingExpiration = true;
//        options.ExpireTimeSpan = TimeSpan.FromHours(5);
//        options.Cookie.HttpOnly = true;

//        options.Cookie.SameSite = SameSiteMode.None;
//        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
//        options.Events = new CookieAuthenticationEvents
//        {
//            OnValidatePrincipal = async context =>
//            {
//                Console.WriteLine($"🔐 Cookie validated at {DateTime.Now}");
//            }
//        };

//    });
// DI Declaration
builder.Services.AddServices(builder.Configuration);

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

app.UseCors("angular");

//miiddleware
app.UseMiddleware<CustomAuthorizeMiddleware>();
app.UseMiddleware<PermissionMiddleware>();
app.UseMiddleware<AutoPermissionMiddleware>();

//app.UseAuthentication();
app.UseAuthorization();



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
