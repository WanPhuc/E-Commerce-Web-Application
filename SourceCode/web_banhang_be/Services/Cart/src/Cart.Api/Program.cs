using AuraMart.Cart.Infrastructure;
using AuraMart.Cart.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using AuraMart.Cart.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<CartDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("CartConnection"));
    if (builder.Environment.IsDevelopment())
    {
        options.EnableSensitiveDataLogging();
        options.EnableDetailedErrors();
    }
});

// Repositories & Services DI
builder.Services.AddScoped<ICartRepository, CartRepository>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<ICommonService, CommonService>();
builder.Services.AddHttpContextAccessor();

// Auth middleware dependencies
builder.Services.AddHttpClient("identity", c => c.BaseAddress = new Uri(builder.Configuration["IdentityApi:BaseUrl"] ?? "http://identity-api:8080"));

// ===== Redis Cache =====
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration["Redis:ConnectionString"];
    options.InstanceName = "Cart_";
});
builder.Services.AddSingleton<BuildingBlocks.Redis.ICacheService, BuildingBlocks.Redis.RedisCacheService>();

builder.Services.AddControllers()
    .AddJsonOptions(o => o.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase);

builder.Services.AddMemoryCache();
builder.Services.AddHealthChecks();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<CartDbContext>();
    await db.Database.EnsureCreatedAsync();
    if (app.Environment.IsDevelopment() || builder.Configuration.GetValue("DemoSeed:Enabled", true))
        await AuraMart.Cart.Infrastructure.Persistence.Seeders.CartDemoSeeder.SeedAsync(db);
}

app.UseMiddleware<ExceptionMiddleware>();
app.UseMiddleware<CustomAuthorizeMiddleware>();
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health");

app.Run();
