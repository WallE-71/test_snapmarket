using System.Threading.Tasks;
using System.Collections.Generic;
using SnapMarket.Entities;
using SnapMarket.ViewModels.Category;

namespace SnapMarket.Data.Contracts
{
    public interface ICategoryRepository
    {
        int CountCategories();
        Category FindByCategoryName(string categoryName);
        Task<List<TreeViewCategory>> GetAllCategoriesAsync();
        bool IsExistCategory(string categoryName, int recentCategoryId = 0);
        Task<List<CategoryViewModel>> GetPaginateCategoriesAsync(int offset, int limit, string orderBy, string searchText);
        Task<List<TreeViewCategory>> GetSubCategoriesByName(string parentName);
    }
}
