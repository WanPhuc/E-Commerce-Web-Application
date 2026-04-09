using Microsoft.EntityFrameworkCore;
using WebBanHang.Data;
using WebBanHang.Helpers.Product;
using WebBanHang.Repositories;
using WebBanHang.Repositories.Interfaces;
using WebBanHang.Repositories.SqlServer;
using WebBanHang.Services.Global.Implements;
using WebBanHang.Services.Global.Interfaces;
using WebBanHang.Services.Implementations;
using WebBanHang.Services.Interfaces;
using WebBanHang.Services.Seller.Implementations;
using WebBanHang.Services.Seller.Interfaces;

namespace WebBanHang
{
    public static class  ServiceConfiguration
    {
        public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(options =>

                options.UseSqlServer(
                    configuration.GetConnectionString("SqlServerConnection")
                )
                .EnableSensitiveDataLogging()
                .EnableDetailedErrors()
            );
            #region Special services
            services.AddScoped(typeof(IRepository<>), typeof(SqlServerRepository<>));
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<ICommonService, CommonService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<FileHelper>();
            #endregion

            #region Service
            //Services - Admin
            services.AddScoped<ISellerService, SellerService>();
            services.AddScoped<ISellerApplicationService, SellerApplicationService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IAdminService, AdminService>();

            //Services - Seller
            services.AddScoped<IProductSellerService, ProductSellerService>();
            services.AddScoped<ISellerInventoryService, SellerInventoryService>();
            services.AddScoped<ISellerOrderService, SellerOrderService>();
            services.AddScoped<ISellerRevenueService, SellerRevenueService>();
            services.AddScoped<ISellerDashboardService, SellerDashboardService>();
            services.AddScoped<ISellerSettingService, SellerSettingService>();
            #endregion

            #region Repositorys
            //Repository
            services.AddScoped<ICartRepository, SqlServerCartRepository>();
            services.AddScoped<IOderRepository, SqlServerOderRepository>();
            services.AddScoped<IProductRepository, SqlServerProductRepository>();
            services.AddScoped<ISellerRepository, SqlServerSellerRepository>();
            services.AddScoped<ISellerApplicationRepository, SqlServerSellerApplicationRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<ICategoryRepository, SqlServerCategoryRepository>();
            services.AddScoped<INotificationRepository, NotificationRepository>();
            #endregion

            return services;
        }
    }
}
