// ============================================================================
// PHASE 6.0 - AuraMart API Gateway (YARP)
//
// Vai tro: cong vao duy nhat cho toan bo he thong.
// Giai doan nay route 100% ve core app (monolith) - chua co service nao tach.
//
// Khi extract tung service (6.1 -> 6.4), chi can:
//   1. Them route moi trong appsettings.json (vd /api/v1/catalog/** -> catalog-svc)
//   2. Route cu cua core app thu hep lai tuong ung
// Client (Angular) chi biet gateway - khong bao hoi doi URL khi tach.
//
// CorrelationId: gateway tu dong truyen tiep header X-Correlation-Id sang
// cac destination vi YARP forward nguyen headers theo mac dinh.
// ============================================================================

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.SetIsOriginAllowed(_ => true)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

builder.Services.AddHealthChecks();

var app = builder.Build();

app.UseCors("AllowAll");

app.MapHealthChecks("/health/live");
app.MapHealthChecks("/health/gateway-ready");

app.MapReverseProxy();

app.Run();
