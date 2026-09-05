using Microsoft.EntityFrameworkCore;
using AuraMart.Catalog.Infrastructure.Persistence;
using BuildingBlocks.Redis;
using VanFucVN.Core.Common.Services;
using VanFucVN.Core.Web.Middlewares;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<CatalogDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("CatalogConnection"));
    if (builder.Environment.IsDevelopment())
    {
        options.EnableSensitiveDataLogging();
        options.EnableDetailedErrors();
    }
});

// ===== DI (tu module Catalog) =====
builder.Services.AddSingleton<WebBanHang.Helpers.Product.FileHelper>();
builder.Services.AddHttpClient<WebBanHang.Services.Interfaces.ISupabaseStorageService, WebBanHang.Services.Implementations.SupabaseStorageService>();
builder.Services.AddScoped<AuraMart.Catalog.Services.ICategoryService, AuraMart.Catalog.Services.CategoryService>();
builder.Services.AddScoped<AuraMart.Catalog.Services.IProductSellerService, AuraMart.Catalog.Services.ProductSellerService>();
builder.Services.AddScoped<AuraMart.Catalog.Services.ISellerInventoryService, AuraMart.Catalog.Services.SellerInventoryService>();
builder.Services.AddScoped<AuraMart.Catalog.Domain.Repositories.IProductRepository, AuraMart.Catalog.Repositories.Implements.ProductRepository>();
builder.Services.AddScoped<AuraMart.Catalog.Domain.Repositories.ICategoryRepository, AuraMart.Catalog.Repositories.Implements.CategoryRepository>();
builder.Services.AddScoped<AuraMart.Catalog.Domain.Repositories.IProductReviewRepository, AuraMart.Catalog.Repositories.Implements.ProductReviewRepository>();

// Auth middleware dependencies: user exists check goi sang IDENTITY service
builder.Services.AddHttpClient("identity", c => c.BaseAddress = new Uri(builder.Configuration["IdentityApi:BaseUrl"] ?? "http://identity-api:8080"));
builder.Services.AddScoped<ICommonService, CommonService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddMemoryCache();
builder.Services.AddSingleton<ICacheService, RedisCacheService>();

// ===== Redis Cache =====
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration["Redis:ConnectionString"];
    options.InstanceName = "Catalog_";
});

// ===== RabbitMQ Connection =====
builder.Services.AddSingleton<RabbitMQ.Client.IConnectionFactory>(sp =>
    new RabbitMQ.Client.ConnectionFactory
    {
        HostName = builder.Configuration["RabbitMQ:HostName"] ?? "localhost",
        Port = int.Parse(builder.Configuration["RabbitMQ:Port"] ?? "5672"),
        UserName = builder.Configuration["RabbitMQ:UserName"] ?? "guest",
        Password = builder.Configuration["RabbitMQ:Password"] ?? "guest"
    });

builder.Services.AddHttpClient("ordering", c => c.BaseAddress = new Uri(builder.Configuration["OrderingApi:BaseUrl"] ?? "http://ordering-api:8084"));
builder.Services.AddHttpClient("core", c => c.BaseAddress = new Uri(builder.Configuration["OrderingApi:BaseUrl"] ?? "http://ordering-api:8084"));
builder.Services.AddScoped<AuraMart.Catalog.Application.ISellerLookup, AuraMart.Catalog.Infrastructure.External.HttpSellerLookup>();

// RabbitMQ subscribers - consume events from Ordering/Payment
builder.Services.AddTransient<BuildingBlocks.EventBus.IIntegrationEventHandler<BuildingBlocks.EventBus.OrderPaidIntegrationEvent>, AuraMart.Catalog.Handlers.OrderPaidHandler>();
builder.Services.AddTransient<BuildingBlocks.EventBus.IIntegrationEventHandler<BuildingBlocks.EventBus.OrderCompletedIntegrationEvent>, AuraMart.Catalog.Handlers.OrderCompletedHandler>();
builder.Services.AddTransient<BuildingBlocks.EventBus.IIntegrationEventHandler<BuildingBlocks.EventBus.OrderCancelledIntegrationEvent>, AuraMart.Catalog.Handlers.OrderCancelledHandler>();
builder.Services.AddHostedService<BuildingBlocks.RabbitMQ.RabbitMQSubscriberService<BuildingBlocks.EventBus.OrderPaidIntegrationEvent>>();
builder.Services.AddHostedService<BuildingBlocks.RabbitMQ.RabbitMQSubscriberService<BuildingBlocks.EventBus.OrderCompletedIntegrationEvent>>();
builder.Services.AddHostedService<BuildingBlocks.RabbitMQ.RabbitMQSubscriberService<BuildingBlocks.EventBus.OrderCancelledIntegrationEvent>>();

builder.Services.AddControllers()
    .AddJsonOptions(o => o.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase);

builder.Services.AddHealthChecks();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<CatalogDbContext>();
    await db.Database.EnsureCreatedAsync();
    if (app.Environment.IsDevelopment() || builder.Configuration.GetValue("DemoSeed:Enabled", true))
        await AuraMart.Catalog.Infrastructure.Persistence.Seeders.CatalogDemoSeeder.SeedAsync(db);
}

app.UseMiddleware<ExceptionMiddleware>();
app.UseMiddleware<CustomAuthorizeMiddleware>();
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health");

string internalApiKey = builder.Configuration["InternalApi:Key"] ?? "dev-internal-key";
AuraMart.Catalog.Controllers.InternalCatalogController.InternalApiKey = internalApiKey;

app.Run();
