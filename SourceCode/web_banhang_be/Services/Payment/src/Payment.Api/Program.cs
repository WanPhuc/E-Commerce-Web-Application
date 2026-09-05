using AuraMart.Payment.Infrastructure;
using AuraMart.Payment.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Serilog;
using AuraMart.Payment.Api.Controllers;
using AuraMart.Payment.Infrastructure.Persistence;
using BuildingBlocks.Outbox;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddDbContext<PaymentDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("PaymentConnection"));
    if (builder.Environment.IsDevelopment())
    {
        options.EnableSensitiveDataLogging();
        options.EnableDetailedErrors();
    }
});

// Repositories & Services DI
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<ICommonService, CommonService>();
builder.Services.AddHttpContextAccessor();

// Auth middleware dependencies
builder.Services.AddHttpClient("identity", c => c.BaseAddress = new Uri(builder.Configuration["IdentityApi:BaseUrl"] ?? "http://identity-api:8080"));

// Outbox infrastructure
builder.Services.AddOutboxDispatcher<PaymentDbContext>("Payment");

// ===== Redis Cache =====
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration["Redis:ConnectionString"];
    options.InstanceName = "Payment_";
});
builder.Services.AddSingleton<BuildingBlocks.Redis.ICacheService, BuildingBlocks.Redis.RedisCacheService>();

// ===== RabbitMQ Connection =====
builder.Services.AddSingleton<RabbitMQ.Client.IConnectionFactory>(sp =>
    new RabbitMQ.Client.ConnectionFactory
    {
        HostName = builder.Configuration["RabbitMQ:HostName"] ?? "localhost",
        Port = int.Parse(builder.Configuration["RabbitMQ:Port"] ?? "5672"),
        UserName = builder.Configuration["RabbitMQ:UserName"] ?? "guest",
        Password = builder.Configuration["RabbitMQ:Password"] ?? "guest"
    });

builder.Services.AddControllers()
    .AddJsonOptions(o => o.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase);

builder.Services.AddMemoryCache();
builder.Services.AddHealthChecks();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<PaymentDbContext>();
    await db.Database.EnsureCreatedAsync();
    if (app.Environment.IsDevelopment() || builder.Configuration.GetValue("DemoSeed:Enabled", true))
        await AuraMart.Payment.Infrastructure.Persistence.Seeders.PaymentDemoSeeder.SeedAsync(db);
}

string apiKey = builder.Configuration["InternalApi:Key"] ?? "dev-internal-key";
InternalPaymentController.InternalApiKey = apiKey;

app.UseMiddleware<ExceptionMiddleware>();
app.UseMiddleware<CustomAuthorizeMiddleware>();
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health");

app.Run();
