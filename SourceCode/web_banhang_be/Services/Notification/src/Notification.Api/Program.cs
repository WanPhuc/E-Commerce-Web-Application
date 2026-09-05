using AuraMart.Notification.Infrastructure;
using AuraMart.Notification.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using AuraMart.Notification.Infrastructure.Persistence;
using AuraMart.Notification.Infrastructure.Messaging;
using AuraMart.Notification.Infrastructure.Realtime;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<NotificationDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("NotificationConnection"));
    if (builder.Environment.IsDevelopment())
    {
        options.EnableSensitiveDataLogging();
        options.EnableDetailedErrors();
    }
});

// Repositories & Services DI
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<ICommonService, CommonService>();
builder.Services.AddHttpContextAccessor();

// Auth middleware dependencies
builder.Services.AddHttpClient("identity", c => c.BaseAddress = new Uri(builder.Configuration["IdentityApi:BaseUrl"] ?? "http://identity-api:8080"));

// ===== SignalR =====
builder.Services.AddSignalR();

// ===== Redis Cache =====
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration["Redis:ConnectionString"];
    options.InstanceName = "Notification_";
});
builder.Services.AddSingleton<BuildingBlocks.Redis.ICacheService, BuildingBlocks.Redis.RedisCacheService>();

// ===== RabbitMQ Connection & Subscribers =====
builder.Services.AddSingleton<RabbitMQ.Client.IConnectionFactory>(sp =>
    new RabbitMQ.Client.ConnectionFactory
    {
        HostName = builder.Configuration["RabbitMQ:HostName"] ?? "localhost",
        Port = int.Parse(builder.Configuration["RabbitMQ:Port"] ?? "5672"),
        UserName = builder.Configuration["RabbitMQ:UserName"] ?? "guest",
        Password = builder.Configuration["RabbitMQ:Password"] ?? "guest"
    });

builder.Services.AddTransient<BuildingBlocks.EventBus.IIntegrationEventHandler<BuildingBlocks.EventBus.OrderPaidIntegrationEvent>, OrderNotificationHandler>();
builder.Services.AddTransient<BuildingBlocks.EventBus.IIntegrationEventHandler<BuildingBlocks.EventBus.OrderCompletedIntegrationEvent>, OrderNotificationHandler>();
builder.Services.AddTransient<BuildingBlocks.EventBus.IIntegrationEventHandler<BuildingBlocks.EventBus.OrderCancelledIntegrationEvent>, OrderNotificationHandler>();

builder.Services.AddHostedService<BuildingBlocks.RabbitMQ.RabbitMQSubscriberService<BuildingBlocks.EventBus.OrderPaidIntegrationEvent>>();
builder.Services.AddHostedService<BuildingBlocks.RabbitMQ.RabbitMQSubscriberService<BuildingBlocks.EventBus.OrderCompletedIntegrationEvent>>();
builder.Services.AddHostedService<BuildingBlocks.RabbitMQ.RabbitMQSubscriberService<BuildingBlocks.EventBus.OrderCancelledIntegrationEvent>>();

builder.Services.AddControllers()
    .AddJsonOptions(o => o.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase);

builder.Services.AddMemoryCache();
builder.Services.AddHealthChecks();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<NotificationDbContext>();
    await db.Database.EnsureCreatedAsync();
    if (app.Environment.IsDevelopment() || builder.Configuration.GetValue("DemoSeed:Enabled", true))
        await AuraMart.Notification.Infrastructure.Persistence.Seeders.NotificationDemoSeeder.SeedAsync(db);
}

app.UseMiddleware<ExceptionMiddleware>();
app.UseMiddleware<CustomAuthorizeMiddleware>();
app.UseAuthorization();

app.MapControllers();
app.MapHub<GlobalHub>("/hubs/global");
app.MapHealthChecks("/health");

app.Run();
