using Microsoft.EntityFrameworkCore;
using AuraMart.Identity.Infrastructure.Persistence;
using WebBanHang.Data.Seeders;



using AuraMart.Identity.Repositories.Implements;
using AuraMart.Identity.Domain.Repositories;
using AuraMart.Identity.Controllers;
using AuraMart.Identity.Application;
using AuraMart.Identity.Infrastructure;



var builder = WebApplication.CreateBuilder(args);

// ===== DbContext rieng cua Identity =====
builder.Services.AddDbContext<IdentityDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("IdentityConnection"));
    if (builder.Environment.IsDevelopment())
    {
        options.EnableSensitiveDataLogging();
        options.EnableDetailedErrors();
    }
});

// ===== DI (copy tu module Identity cua monolith) =====
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
builder.Services.AddScoped<ICommonService, CommonService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddMemoryCache();

// ===== Redis Cache =====
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration["Redis:ConnectionString"];
    options.InstanceName = "Identity_";
});

builder.Services.AddSingleton<BuildingBlocks.Redis.ICacheService, BuildingBlocks.Redis.RedisCacheService>();
builder.Services.AddHttpClient("core", c => c.BaseAddress = new Uri(builder.Configuration["CoreApi:BaseUrl"] ?? "http://api:8080"));

builder.Services.AddControllers()
    .AddJsonOptions(o => o.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase);

builder.Services.AddHealthChecks();

var app = builder.Build();

// ===== Seed (roles + admin + demo identities) =====
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<IdentityDbContext>();
    await db.Database.EnsureCreatedAsync();
    await RoleSeeder.SeedAsync(db);
    await UserSeeder.SeedAsync(db);

    if (app.Environment.IsDevelopment() || builder.Configuration.GetValue<bool>("DemoSeed:Enabled", true))
    {
        await WebBanHang.Data.Seeders.DemoIdentitySeeder.SeedAsync(db);
    }
}

app.UseMiddleware<ExceptionMiddleware>();
app.UseMiddleware<CustomAuthorizeMiddleware>();
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health");

// ===== Internal API cho cac service khac (Phase 6.1) =====
// Bao mat service-to-service: header X-Internal-Api-Key phai khop config.
string internalApiKey = builder.Configuration["InternalApi:Key"] ?? "dev-internal-key";
InternalUserController.InternalApiKey = internalApiKey;

app.Run();
