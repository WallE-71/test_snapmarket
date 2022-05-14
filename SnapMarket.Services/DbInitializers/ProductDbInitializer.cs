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
using System.Collections.Generic;

namespace SnapMarket.Services.DbInitializers
{
    public class ProductDbInitializer : IProductDbInitializer
    {
        private IUnitOfWork _uw;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<ProductDbInitializer> _logger;
        public ProductDbInitializer(
            IUnitOfWork uw,
            IServiceScopeFactory scopeFactory,
            ILogger<ProductDbInitializer> logger)
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
                var dbInitializer = serviceScope.ServiceProvider.GetService<IProductDbInitializer>();
                var result = dbInitializer.SeedDatabaseAsync().Result;

                if (result == IdentityResult.Failed())
                    throw new InvalidOperationException(result.DumpErrors());
            }
        }

        public async Task<IdentityResult> SeedDatabaseAsync()
        {
            var product = new Product();
            var thisMethodName = nameof(SeedDatabaseAsync);
            product = await _uw.Context.Products.FirstOrDefaultAsync(p => p.Name == "شامپو انار پرژک 450 گرمی" && p.SellerId == 1);
            if (product != null)
            {
                _logger.LogInformation($"{thisMethodName}: this Product already exists.");
                return IdentityResult.Success;
            }

            var productId = StringExtensions.GenerateId(10);
            var productCategories = new List<ProductCategory>();
            productCategories.Add(new ProductCategory { CategoryId = 1, ProductId = productId });
            _uw.Context.Products.AddRange(
            product = new Product
            {
                BrandId = 1,
                SellerId = 1,
                Stock = 1000,
                Price = 20000,
                Id = productId,
                IsPrefered = false,
                States = ProductState.Demo,
                Name = "شامپو انار پرژک 450 گرمی",
                Description = "<p>Nice Product</p>",
                ProductCategories = productCategories,
                ExpirationDate = DateTime.Today.AddYears(1),
                InsertTime = DateTimeExtensions.DateTimeWithOutMilliSecends(DateTime.Now.Date)
            });
            await _uw.BaseRepository<Product>().CreateAsync(product);
            await _uw.Commit();
            return IdentityResult.Failed();
        }
    }
}
