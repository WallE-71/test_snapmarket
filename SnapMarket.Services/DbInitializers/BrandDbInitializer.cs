using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SnapMarket.Data;
using SnapMarket.Entities;
using SnapMarket.Data.Contracts;
using SnapMarket.Common.Extensions;
using SnapMarket.Services.Contracts;

namespace SnapMarket.Services.DbInitializers
{
    public class BrandDbInitializer : IBrandDbInitializer
    {
        private IUnitOfWork _uw;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<BrandDbInitializer> _logger;
        public BrandDbInitializer(
            IUnitOfWork uw,
            IServiceScopeFactory scopeFactory,
             ILogger<BrandDbInitializer> logger)
        {
            _uw = uw;
            _logger = logger;
            _scopeFactory = scopeFactory;
        }

        public void Initialize()
        {
            using (var serviceScope = _scopeFactory.CreateScope())
            {
                using (var context = serviceScope.ServiceProvider.GetService<SnapMarketDBContext>())
                {
                    context.Database.Migrate();
                }
            }
        }

        /// <summary>
        /// Adds some default values to the IdentityDb
        /// </summary>
        public void SeedData()
        {
            using (var serviceScope = _scopeFactory.CreateScope())
            {
                var dbInitializer = serviceScope.ServiceProvider.GetService<IBrandDbInitializer>();
                var result = dbInitializer.SeedDatabaseAsync().Result;

                if (result == IdentityResult.Failed())
                    throw new InvalidOperationException(result.DumpErrors());
            }
        }

        public async Task<IdentityResult> SeedDatabaseAsync()
        {
            var thisMethodName = nameof(SeedDatabaseAsync);
            var brand = _uw.Context.Brands.Where(s => s.Id == 1).FirstOrDefault();
            if (brand != null)
            {
                _logger.LogInformation($"{thisMethodName}: this Seller already exists.");
                return IdentityResult.Success;
            }

            _uw.Context.Brands.AddRange(
            brand = new Brand
            {
                Id = 1,
                Name = "پرژک",
                InsertTime = DateTime.Now,
            });
            await _uw.BaseRepository<Brand>().CreateAsync(brand);
            await _uw.Commit();
            return IdentityResult.Success;
        }
    }
}
