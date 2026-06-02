using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Events;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Security.Cryptography.Xml;
using System.Text.Json;
using System.Text.Json.Serialization;
using WebBanHang;
using WebBanHang.Data;
using WebBanHang.Data.Seeders;
using WebBanHang.Filters;
using WebBanHang.Helpers;
using WebBanHang.Helpers.Hubs;
using WebBanHang.Helpers.Product;
using WebBanHang.Middleware;
using WebBanHang.Repositories;
using WebBanHang.Repositories.SqlServer;
using WebBanHang.Services.Implementations;
using WebBanHang.Services.Interfaces;
using WebBanHang.Services.Seller.Implementations;
using WebBanHang.Services.Seller.Interfaces;

// Xóa bỏ việc tự động map claim của Microsoft để giữ nguyên tên gốc (sub, email, jti...)
JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

var builder = WebApplication.CreateBuilder(args);

// Serilog
var appLogTemplate = "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {SourceContext:l} - {Message:lj}{NewLine}{Exception}";
var errorLogTemplate =
    "--------------------------------------------------------------------------------{NewLine}" +
    "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {SourceContext:l}{NewLine}" +
    "Message : {Message:lj}{NewLine}" +
    "Trace   : {TraceId}{NewLine}" +
    "Span    : {SpanId}{NewLine}" +
    "{Exception}{NewLine}";
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()//Set level mặc định thấp nhất là Information. Nghĩa là log từ Information trở lên sẽ được ghi: Information, Warning, Error, Fatal
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)//Giảm log rác từ thư viện Microsoft
    .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Database.Command", LogEventLevel.Information)//log chi tiết các câu lệnh SQL từ Entity Framework Core
    .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Infrastructure", LogEventLevel.Information)//log chi tiết các hoạt động của EF Core như kết nối, truy vấn, thay đổi trạng thái đối tượng
    .Enrich.FromLogContext()//Thêm thông tin ngữ cảnh vào log, ví dụ như TraceId, SpanId để dễ dàng theo dõi luồng thực thi
    .WriteTo.Logger(lc => lc
        .Filter.ByExcluding(evt =>
            evt.Properties.TryGetValue("SourceContext", out var source)
            && source.ToString().Contains("Microsoft.EntityFrameworkCore.Database.Command"))
        .Filter.ByIncludingOnly(evt =>
            evt.Level == LogEventLevel.Information || evt.Level == LogEventLevel.Warning)
        .WriteTo.File(
            path: "Logs/app-.txt",
            rollingInterval: RollingInterval.Day,//moi ngay 1 file log
            retainedFileCountLimit: 30,//giữ lại tối đa 30 file log, tức là log của 30 ngày gần nhất
            fileSizeLimitBytes: 10_000_000,//giới hạn kích thước mỗi file log là 10MB, nếu vượt quá sẽ tạo file mới
            rollOnFileSizeLimit: true,//khi file log đạt đến kích thước giới hạn sẽ tự động tạo file mới để tiếp tục ghi log
            shared: true,
            outputTemplate: appLogTemplate
            )
        )
    //File log loi:Error, Fatal
    .WriteTo.Logger(lc => lc
       .Filter.ByIncludingOnly(evt =>
            evt.Level == LogEventLevel.Error || evt.Level == LogEventLevel.Fatal)
        .WriteTo.File(
            path: "Logs/errors-.txt",
            rollingInterval: RollingInterval.Day,
            retainedFileCountLimit: 30,
            fileSizeLimitBytes: 10_000_000,
            rollOnFileSizeLimit: true,
            shared: true,
            outputTemplate: errorLogTemplate
        )
    )
    .CreateLogger();

//cho asp.net sử dụng serilog
builder.Host.UseSerilog();
//log app start
Log.Information("Start {ApplicationName} version {Version} in {Environment} environment",
    builder.Environment.ApplicationName,
    Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "1.0.0",
    builder.Environment.EnvironmentName
);




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
    options.OperationFilter<AllowAnonymousOperationFilter>();
});

builder.Services.AddSignalR();



builder.Services.AddCors(opt =>
{
    opt.AddPolicy("angular", p => p
    .WithOrigins("http://localhost:4200", "https://localhost:4200")
    .AllowAnyHeader()
    .AllowAnyMethod()
    .AllowCredentials());
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddCookie()
.AddGoogle("google", options =>
{
    options.ClientId = builder.Configuration["Authentication:Google:ClientId"];
    options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
});

builder.Services.AddMemoryCache();
// DI Declaration
builder.Services.AddServices(builder.Configuration, builder.Environment);

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

if (builder.Configuration.GetValue<bool>("UseHttpsRedirection", true))
{
    app.UseHttpsRedirection();
}//app.UseStatusCodePagesWithReExecute("/Login/AccessDenied", "?code={0}");
app.UseRouting();

app.UseCors("angular");

//miiddleware
app.UseMiddleware<ExceptionMiddleware>();

app.UseAuthentication();
app.UseMiddleware<CustomAuthorizeMiddleware>();
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
