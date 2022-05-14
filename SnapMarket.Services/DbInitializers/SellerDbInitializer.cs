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
    public class SellerDbInitializer : ISellerDbInitializer
    {
        private IUnitOfWork _uw;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<SellerDbInitializer> _logger;
        public SellerDbInitializer(
            IUnitOfWork uw,
            IServiceScopeFactory scopeFactory,
             ILogger<SellerDbInitializer> logger)
        {
            _uw = uw;
            _scopeFactory = scopeFactory;
            _logger = logger;
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
                var dbInitializer = serviceScope.ServiceProvider.GetService<ISellerDbInitializer>();
                var result = dbInitializer.SeedDatabaseAsync().Result;

                if (result == IdentityResult.Failed())
                    throw new InvalidOperationException(result.DumpErrors());
            }
        }

        public async Task<IdentityResult> SeedDatabaseAsync()
        {
            var thisMethodName = nameof(SeedDatabaseAsync);
            var seller = _uw.Context.Sellers.Where(s => s.Id == 1).FirstOrDefault();
            if (seller != null)
            {
                _logger.LogInformation($"{thisMethodName}: this Seller already exists.");
                return IdentityResult.Success;
            }

            _uw.Context.Sellers.AddRange(
            seller = new Seller
            {
                Id = 1,
                IsComplete = true,
                Name = "Iran Kala",
                Description = "ایران کالا برترین های ایران",
                InsertTime = DateTimeExtensions.DateTimeWithOutMilliSecends(DateTime.Now.Date)
            });
            await _uw.BaseRepository<Seller>().CreateAsync(seller);
            await _uw.Commit();
            return IdentityResult.Success;
        }
    }
}
