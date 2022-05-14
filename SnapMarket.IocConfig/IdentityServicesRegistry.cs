using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using SnapMarket.Services.Identity;
using SnapMarket.Services.Contracts;
using SnapMarket.IocConfig.Extensions;
using SnapMarket.Services.DbInitializers;

namespace SnapMarket.IocConfig
{
    public static class IdentityServicesRegistry
    {
        public static void AddCustomIdentityServices(this IServiceCollection services)
        {
            services.AddIdentityOptions();
            services.AddDynamicPersmission();
            services.AddScoped<IApplicationUserManager, ApplicationUserManager>();
            services.AddScoped<IApplicationRoleManager, ApplicationRoleManager>();
            services.AddScoped<ApplicationIdentityErrorDescriber>();
            services.AddTransient<IIdentityDbInitializer, IdentityDbInitializer>();
            services.AddTransient<ISellerDbInitializer, SellerDbInitializer>();
            services.AddTransient<ICategoriesDbInitializer, CategoriesDbInitializer>();
            services.AddTransient<IBrandDbInitializer, BrandDbInitializer>();
            services.AddTransient<IProductDbInitializer, ProductDbInitializer>();
            services.AddScoped(typeof(ISignInOption<>), typeof(SignInOption<>));
        }

        public static void UseCustomIdentityServices(this IApplicationBuilder app)
        {
            app.UseAuthentication();
            app.CallDbInitializer();
        }

        private static void CallDbInitializer(this IApplicationBuilder app)
        {
            var scopeFactory = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>();
            using (var scope = scopeFactory.CreateScope())
            {
                var identityDbInitializer = scope.ServiceProvider.GetService<IIdentityDbInitializer>();
                var sellerDbInitializer = scope.ServiceProvider.GetService<ISellerDbInitializer>();
                var categoriesDbInitializer = scope.ServiceProvider.GetService<ICategoriesDbInitializer>();
                var brandDbInitializer = scope.ServiceProvider.GetService<IBrandDbInitializer>();
                var productDbInitializer = scope.ServiceProvider.GetService<IProductDbInitializer>();

                identityDbInitializer.Initialize();
                identityDbInitializer.SeedData();

                sellerDbInitializer.Initialize();
                sellerDbInitializer.SeedData();

                categoriesDbInitializer.Initialize();
                categoriesDbInitializer.SeedData();

                brandDbInitializer.Initialize();
                brandDbInitializer.SeedData();

                productDbInitializer.Initialize();
                productDbInitializer.SeedData();
            }
        }
    }
}
