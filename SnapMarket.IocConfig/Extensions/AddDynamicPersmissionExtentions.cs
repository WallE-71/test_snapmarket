using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using SnapMarket.Services;
using SnapMarket.Services.Contracts;

namespace SnapMarket.IocConfig.Extensions
{
    public static class AddDynamicPersmissionExtentions
    {
        public static IServiceCollection AddDynamicPersmission(this IServiceCollection services)
        {
            services.AddSingleton<IAuthorizationHandler, DynamicPermissionsAuthorizationHandler>();
            services.AddSingleton<IMvcActionsDiscoveryService, MvcActionsDiscoveryService>();
            services.AddSingleton<ISecurityTrimmingService, SecurityTrimmingService>();

            return services;
        }
    }
}
