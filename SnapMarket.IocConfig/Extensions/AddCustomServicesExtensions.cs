using Microsoft.Extensions.DependencyInjection;
using SnapMarket.Common;
using SnapMarket.Services;
using SnapMarket.Services.Api;
using SnapMarket.Data.Contracts;
using SnapMarket.Data.UnitOfWork;
using SnapMarket.Services.Contracts;
using SnapMarket.Services.Api.Contract;

namespace SnapMarket.IocConfig.Extensions
{
    public static class AddCustomServicesExtensions
    {
        public static IServiceCollection AddCustomServices(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IEmailSender, EmailSender>();
            services.AddScoped<ISmsSender, SmsSender>();
            services.AddTransient<SendWeeklyNewsLetter>();
            services.AddTransient<IJwtService, JwtService>();

            return services;
        }
    }
}
