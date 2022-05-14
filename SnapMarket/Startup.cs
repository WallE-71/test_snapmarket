using System.IO;
using System.Linq;
using Coravel;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.DependencyInjection;
using SnapMarket.Data;
using SnapMarket.Services;
using SnapMarket.IocConfig;
using SnapMarket.ViewModels.Settings;
using SnapMarket.IocConfig.Extensions;
using SnapMarket.IocConfig.Api.Swagger;
using SnapMarket.ViewModels.DynamicAccess;
using SnapMarket.IocConfig.Api.Middlewares;

namespace SnapMarket
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        private readonly SiteSettings SiteSettings;
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            SiteSettings = configuration.GetSection(nameof(SiteSettings)).Get<SiteSettings>();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<SiteSettings>(Configuration.GetSection(nameof(SiteSettings)));
            services.AddDbContext<SnapMarketDBContext>(options => options.UseSqlServer(Configuration.GetConnectionString("SqlServer")));
            services.AddCustomServices();
            services.AddCustomIdentityServices();
            services.AddAutoMapper();
            services.AddApiVersioning();
            services.AddSwagger();
            services.AddScheduler();
            services.AddCustomAuthentication(SiteSettings);
            services.ConfigureWritable<SiteSettings>(Configuration.GetSection("SiteSettings"));
            services.AddMvc();

            services.AddAuthorization(options =>
            {
                options.AddPolicy(ConstantPolicies.DynamicPermission, policy => policy.Requirements.Add(new DynamicPermissionRequirement()));
            });

            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Error";
                options.AccessDeniedPath = "/Admin/Manage/AccessDenied";
            });

            services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                {
                    var frontendURL = Configuration.GetValue<string>("Frontend_Url");
                    builder.WithOrigins(frontendURL).AllowAnyMethod().AllowAnyHeader()
                           .WithExposedHeaders(new string[] { "totalAmountOfRecords" });
                });

                //options.AddPolicy("", builder =>
                //{
                //    builder.WithOrigins("https://sandbox.zarinpal.com/pg/StartPay/")
                //           .AllowAnyMethod().AllowAnyHeader().WithMethods("PUT", "DELETE", "GET", "POST");
                //});
            });

            services.AddMvc().AddRazorPagesOptions(options =>
            {
                var a = options.Conventions.AddPageRoute("/../Areas/Admin/Manage/SignIn", "");
            });

            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            //services.AddControllers().AddNewtonsoftJson(
            //    options => {
            //    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            //});

            //services.AddControllersWithViews();
            //services.AddRazorPages();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseWhen(context => context.Request.Path.StartsWithSegments("/api") || context.Request.Path.StartsWithSegments("/docs"), appBuilder =>
            {
                appBuilder.UseCustomExceptionHandler();
            });
            app.UseWhen(context => !context.Request.Path.StartsWithSegments("/api") || !context.Request.Path.StartsWithSegments("/docs"), appBuilder =>
            {
                if (env.IsDevelopment())
                    app.UseDeveloperExceptionPage();
                else
                    app.UseExceptionHandler("/Error404");
            });

            var cachePeriod = env.IsDevelopment() ? "600" : "605800";
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "CacheFiles")),
                OnPrepareResponse = ctx =>
                {
                    ctx.Context.Response.Headers.Append("Cache-Control", $"public,max-age={cachePeriod}");
                },
                RequestPath = "/CacheFiles",
            });

            app.UseHttpsRedirection();
            //app.UseBlazorFrameworkFiles();
            app.UseStaticFiles();
            app.UseCustomIdentityServices();

            var provider = app.ApplicationServices;
            provider.UseScheduler(scheduler =>
            {
                scheduler.Schedule<SendWeeklyNewsLetter>().EveryFifteenMinutes()/*.Cron("29 20 * * 5")*/;
            });

            app.Use(async (context, next) =>
            {
                await next();
                if (context.Response.StatusCode == 404)
                {
                    context.Request.Path = "/Error404";
                    await next();
                }
            });

            app.UseSwaggerAndUI();
            app.UseRouting();
            app.UseCors();
            app.UseAuthorization();
            //app.UseEasyAuth2();

            app.UseEndpoints(endpoints =>
            {
                //endpoints.MapRazorPages();
                //endpoints.MapControllers();
                //endpoints.MapFallbackToFile("index.html");

                endpoints.MapControllerRoute(
                    name: "areas",
                    pattern: "{area:exists}/{controller=Manage}/{action=SignIn}"
                );
            });
        }
    }

    //public static class ApplicationBuilderExtension
    //{
    //    public static void UseEasyAuth2(this IApplicationBuilder app)
    //    {
    //        app.Use(async (context, next) =>
    //        {
    //            if (context.Request.Headers.ContainsKey("X-MS-CLIENT-PRINCIPAL-ID"))
    //            {
    //                var azureAppServicePrincipalIdHeader = context.Request.Headers["X-MS-CLIENT-PRINCIPAL-ID"][0];
    //                var azureAppServicePrincipalNameHeader = context.Request.Headers["X-MS-CLIENT-PRINCIPAL-NAME"][0];

    //                var cookieContainer = new CookieContainer();
    //                var handler = new HttpClientHandler()
    //                {
    //                    CookieContainer = cookieContainer
    //                };
    //                var uriString = $"{context.Request.Scheme}://{context.Request.Host}";
    //                foreach (var c in context.Request.Cookies)
    //                {
    //                    cookieContainer.Add(new Uri(uriString), new Cookie(c.Key, c.Value));
    //                }

    //                string jsonResult;
    //                using (HttpClient client = new HttpClient(handler))
    //                {
    //                    var res = await client.GetAsync($"{uriString}/.auth/me");
    //                    jsonResult = await res.Content.ReadAsStringAsync();
    //                }

    //                var obj = JArray.Parse(jsonResult);

    //                var user_id = obj[0]["user_id"].Value<string>(); //user_id

    //                var claims = new List<Claim>();
    //                foreach (var claim in obj[0]["user_claims"])
    //                {
    //                    claims.Add(new Claim(claim["typ"].ToString(), claim["val"].ToString()));
    //                }

    //                var identity = new GenericIdentity(azureAppServicePrincipalIdHeader);
    //                identity.AddClaims(claims);

    //                context.User = new GenericPrincipal(identity, null);
    //            }
    //            await next.Invoke();
    //        });
    //    }
    //}
}
