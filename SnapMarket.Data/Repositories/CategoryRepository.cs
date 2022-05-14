using System.Linq;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using System.Collections.Generic;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SnapMarket.Entities;
using SnapMarket.Data.Contracts;
using SnapMarket.Common.Extensions;
using SnapMarket.ViewModels.Category;

namespace SnapMarket.Data.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly SnapMarketDBContext _context;
        public CategoryRepository(SnapMarketDBContext context)
        {
            _context = context;
            _context.CheckArgumentIsNull(nameof(_context));
        }

        public async Task<List<CategoryViewModel>> GetPaginateCategoriesAsync(int offset, int limit, string orderBy, string searchText)
        {
            List<CategoryViewModel> categories = await _context.Categories
                                .GroupJoin(_context.Categories,
                                (cl => cl.ParentId),
                                (or => or.Id),
                                ((cl, or) => new { CategoryInfo = cl, ParentInfo = or }))
                                .SelectMany(p => p.ParentInfo.DefaultIfEmpty(), (x, y) => new { x.CategoryInfo, ParentInfo = y })
                                .OrderBy(orderBy)
                                .Skip(offset).Take(limit)
                                .Select(c => new CategoryViewModel
                                {
                                    Id = c.CategoryInfo.Id,
                                    Name = c.CategoryInfo.Name,
                                    ParentId = c.ParentInfo.Id,
                                    ParentName = c.ParentInfo.Name
                                }).AsNoTracking().ToListAsync();

            foreach (var item in categories)
                item.Row = ++offset;

            return categories;
        }

        public async Task<List<TreeViewCategory>> GetAllCategoriesAsync()
        {
            var Categories = await (from c in _context.Categories
                                    where (c.ParentId == 0 || c.ParentId == null)
                                    select new TreeViewCategory { id = c.Id, title = c.Name }).ToListAsync();

            foreach (var item in Categories)
                BindSubCategories(item);

            return Categories;
        }

        public void BindSubCategories(TreeViewCategory category)
        {
            var SubCategories = (from c in _context.Categories
                                 where (c.ParentId == category.id)
                                 select new TreeViewCategory { id = c.Id, title = c.Name }).ToList();

            foreach (var item in SubCategories)
            {
                BindSubCategories(item);
                category.subs.Add(item);
            }
        }

        public Category FindByCategoryName(string categoryName)
        {
            return _context.Categories.Where(c => c.Name == categoryName.TrimStart().TrimEnd()).FirstOrDefault();
        }

        public bool IsExistCategory(string categoryName, int recentCategoryId = 0)
        {
            if (recentCategoryId == 0)
                return _context.Categories.Any(c => c.Name.Trim().Replace(" ", "") == categoryName.Trim().Replace(" ", ""));
            else
            {
                var category = _context.Categories.Where(c => c.Name.Trim().Replace(" ", "") == categoryName.Trim().Replace(" ", "")).FirstOrDefault();
                if (category == null)
                    return false;
                else
                {
                    if (category.Id != recentCategoryId)
                        return true;
                    else
                        return false;
                }
            }
        }

        public async Task<List<TreeViewCategory>> GetSubCategoriesByName(string parentName)
        {
            var subCategories = await (from c in _context.Categories.Include(c => c.SubCategories).Where(n => n.Name == parentName)
                                      select (new TreeViewCategory
                                      {
                                          id = c.Id,
                                          title = c.Name
                                      })).AsNoTracking().ToListAsync();
            
            var _subCategories = new List<TreeViewCategory>();
            var treeViewCategories = new List<TreeViewCategory>();
            foreach (var item in subCategories)
            {
                BindSubCategories(item);
                var viewModel = new TreeViewCategory();
                var treeView = subCategories.FirstOrDefault(c => c.id == item.id);
                viewModel.title = treeView.title;
                if (item.subs.Count() != 0)
                {
                    foreach (var sub in item.subs)
                    {
                        var subCategory = new TreeViewCategory();
                        subCategory.title = sub.title;
                        _subCategories.Add(subCategory);
                    }
                }
                viewModel.subs = _subCategories;
                treeViewCategories.Add(viewModel);
            }
            return treeViewCategories;
        }

        public int CountCategories() => _context.Categories.Count();
    }
}
