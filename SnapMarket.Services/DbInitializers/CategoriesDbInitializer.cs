using System;
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
    public class CategoriesDbInitializer : ICategoriesDbInitializer
    {
        private IUnitOfWork _uw;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<CategoriesDbInitializer> _logger;
        public CategoriesDbInitializer(
            IUnitOfWork uw,
            IServiceScopeFactory scopeFactory,
             ILogger<CategoriesDbInitializer> logger)
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
                var dbInitializer = serviceScope.ServiceProvider.GetService<ICategoriesDbInitializer>();
                var result = dbInitializer.SeedDatabaseAsync().Result;

                if (result == IdentityResult.Failed())
                    throw new InvalidOperationException(result.DumpErrors());
            }
        }

        public async Task<IdentityResult> SeedDatabaseAsync()
        {
            try
            {

                var thisMethodName = nameof(SeedDatabaseAsync);
                var category1 = _uw.CategoryRepository.FindByCategoryName("لبنیات");
                var category2 = _uw.CategoryRepository.FindByCategoryName("خواربار و نان");
                var category3 = _uw.CategoryRepository.FindByCategoryName("دستمال و شوینده");
                var category4 = _uw.CategoryRepository.FindByCategoryName("تنقلات");
                var category5 = _uw.CategoryRepository.FindByCategoryName("نوشیدنی");
                var category6 = _uw.CategoryRepository.FindByCategoryName("مواد پروتئینی");
                var category7 = _uw.CategoryRepository.FindByCategoryName("آرایشی و بهداشتی");
                var category8 = _uw.CategoryRepository.FindByCategoryName("چاشنی و افزودنی");
                var category9 = _uw.CategoryRepository.FindByCategoryName("میوه و سبزیجات تازه");
                var category10 = _uw.CategoryRepository.FindByCategoryName("کنسرو و غذای آماده");
                var category11 = _uw.CategoryRepository.FindByCategoryName("صبحانه");
                var category12 = _uw.CategoryRepository.FindByCategoryName("خشکبار، دسر و شیرینی");
                var category13 = _uw.CategoryRepository.FindByCategoryName("خانه و سبک زندگی");
                var category14 = _uw.CategoryRepository.FindByCategoryName("کودک و نوزاد");
                var category15 = _uw.CategoryRepository.FindByCategoryName("مد و پوشاک");
                if (category1 != null && category2 != null && category3 != null && category4 != null && category5 != null && category6 != null && category7 != null && category8 != null && category9 != null && category10 != null && category11 != null && category12 != null && category13 != null && category14 != null && category15 != null)
                {
                    _logger.LogInformation($"{thisMethodName}: this Category already exists.");
                    return IdentityResult.Success;
                }

                _uw.Context.Categories.AddRange(
                new Category
                {
                    Id = 1,
                    Name = "لبنیات",
                    ParentId = null,
                },
                new Category
                {
                    Id = 2,
                    Name = "خواربار و نان",
                    ParentId = null,
                },
                new Category
                {
                    Id = 3,
                    Name = "دستمال و شوینده",
                    ParentId = null,
                },
                new Category
                {
                    Id = 4,
                    Name = "تنقلات",
                    ParentId = null,
                },
                new Category
                {
                    Id = 5,
                    Name = "نوشیدنی",
                    ParentId = null,
                },
                new Category
                {
                    Id = 6,
                    Name = "مواد پروتئینی",
                    ParentId = null,
                },
                new Category
                {
                    Id = 7,
                    Name = "آرایشی و بهداشتی",
                    ParentId = null,
                },
                new Category
                {
                    Id = 8,
                    Name = "چاشنی و افزودنی",
                    ParentId = null,
                },
                new Category
                {
                    Id = 9,
                    Name = "میوه و سبزیجات تازه",
                    ParentId = null,
                },
                new Category
                {
                    Id = 10,
                    Name = "کنسرو و غذای آماده",
                    ParentId = null,
                },
                new Category
                {
                    Id = 11,
                    Name = "صبحانه",
                    ParentId = null,
                },
                new Category
                {
                    Id = 12,
                    Name = "خشکبار، دسر و شیرینی",
                    ParentId = null,
                },
                new Category
                {
                    Id = 13,
                    Name = "خانه و سبک زندگی",
                    ParentId = null,
                },
                new Category
                {
                    Id = 14,
                    Name = "کودک و نوزاد",
                    ParentId = null,
                },
                new Category
                {
                    Id = 15,
                    Name = "مد و پوشاک",
                    ParentId = null,
                });

                await _uw.Commit();
                return IdentityResult.Success;
            }
            catch(Exception e) { }
            return new IdentityResult();
        }
    }
}
