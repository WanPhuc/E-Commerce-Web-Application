using Microsoft.EntityFrameworkCore;
using Serilog;
using AuraMart.Ordering.Api.Controllers;
using AuraMart.Ordering.Infrastructure.Persistence;
using BuildingBlocks.Outbox;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddDbContext<OrderingDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("OrderingConnection"));
    if (builder.Environment.IsDevelopment())
    {
        options.EnableSensitiveDataLogging();
        options.EnableDetailedErrors();
    }
});

// Repositories & Services DI
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<ICommonService, CommonService>();
builder.Services.AddHttpContextAccessor();

// Auth middleware dependencies
builder.Services.AddHttpClient("identity", c => c.BaseAddress = new Uri(builder.Configuration["IdentityApi:BaseUrl"] ?? "http://identity-api:8080"));

// Outbox infrastructure
builder.Services.AddOutboxDispatcher<OrderingDbContext>("Ordering");

// ===== Redis Cache =====
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration["Redis:ConnectionString"];
    options.InstanceName = "Ordering_";
});
builder.Services.AddSingleton<BuildingBlocks.Redis.ICacheService, BuildingBlocks.Redis.RedisCacheService>();

// ===== RabbitMQ =====
builder.Services.AddSingleton<RabbitMQ.Client.IConnectionFactory>(sp =>
    new RabbitMQ.Client.ConnectionFactory
    {
        HostName = builder.Configuration["RabbitMQ:HostName"] ?? "localhost",
        Port = int.Parse(builder.Configuration["RabbitMQ:Port"] ?? "5672"),
        UserName = builder.Configuration["RabbitMQ:UserName"] ?? "guest",
        Password = builder.Configuration["RabbitMQ:Password"] ?? "guest"
    });

// RabbitMQ subscribers — consume PaymentSucceeded/PaymentFailed events
builder.Services.AddTransient<BuildingBlocks.EventBus.IIntegrationEventHandler<BuildingBlocks.EventBus.PaymentSucceededIntegrationEvent>, AuraMart.Ordering.Api.Handlers.PaymentSucceededHandler>();
builder.Services.AddTransient<BuildingBlocks.EventBus.IIntegrationEventHandler<BuildingBlocks.EventBus.PaymentFailedIntegrationEvent>, AuraMart.Ordering.Api.Handlers.PaymentFailedHandler>();
builder.Services.AddHostedService<BuildingBlocks.RabbitMQ.RabbitMQSubscriberService<BuildingBlocks.EventBus.PaymentSucceededIntegrationEvent>>();
builder.Services.AddHostedService<BuildingBlocks.RabbitMQ.RabbitMQSubscriberService<BuildingBlocks.EventBus.PaymentFailedIntegrationEvent>>();

builder.Services.AddControllers()
    .AddJsonOptions(o => o.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase);

builder.Services.AddMemoryCache();
builder.Services.AddHealthChecks();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<OrderingDbContext>();
    await db.Database.EnsureCreatedAsync();
    if (app.Environment.IsDevelopment() || builder.Configuration.GetValue("DemoSeed:Enabled", true))
        await AuraMart.Ordering.Infrastructure.Persistence.Seeders.OrderingDemoSeeder.SeedAsync(db);
}

string apiKey = builder.Configuration["InternalApi:Key"] ?? "dev-internal-key";
InternalOrderingController.InternalApiKey = apiKey;

app.UseMiddleware<ExceptionMiddleware>();
app.UseMiddleware<CustomAuthorizeMiddleware>();
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health");

app.Run();
