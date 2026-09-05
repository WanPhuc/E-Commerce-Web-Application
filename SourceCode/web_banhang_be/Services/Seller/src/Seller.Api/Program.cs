using AuraMart.Seller.Infrastructure;
using AuraMart.Seller.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Serilog;
using AuraMart.Seller.Api.Controllers;
using AuraMart.Seller.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddDbContext<SellerDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("SellerConnection"));
    if (builder.Environment.IsDevelopment())
    {
        options.EnableSensitiveDataLogging();
        options.EnableDetailedErrors();
    }
});

// Repositories & Services DI
builder.Services.AddScoped<ISellerRepository, SellerRepository>();
builder.Services.AddScoped<ISellerApplicationRepository, SellerApplicationRepository>();
builder.Services.AddScoped<ISellerService, SellerService>();
builder.Services.AddScoped<ICommonService, CommonService>();
builder.Services.AddHttpContextAccessor();

// Auth middleware dependencies
builder.Services.AddHttpClient("identity", c => c.BaseAddress = new Uri(builder.Configuration["IdentityApi:BaseUrl"] ?? "http://identity-api:8080"));

// ===== Redis Cache =====
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration["Redis:ConnectionString"];
    options.InstanceName = "Seller_";
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
    var db = scope.ServiceProvider.GetRequiredService<SellerDbContext>();
    await db.Database.EnsureCreatedAsync();
    if (app.Environment.IsDevelopment() || builder.Configuration.GetValue("DemoSeed:Enabled", true))
        await AuraMart.Seller.Infrastructure.Persistence.Seeders.SellerDemoSeeder.SeedAsync(db);
}

string apiKey = builder.Configuration["InternalApi:Key"] ?? "dev-internal-key";
InternalSellerController.InternalApiKey = apiKey;

app.UseMiddleware<ExceptionMiddleware>();
app.UseMiddleware<CustomAuthorizeMiddleware>();
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health");

app.Run();
