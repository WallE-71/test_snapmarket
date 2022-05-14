using System.Collections.Generic;
using SnapMarket.ViewModels.Category;

namespace SnapMarket.ViewModels.Product
{
    public class ProductCategoriesViewModel
    {
        public ProductCategoriesViewModel(List<TreeViewCategory> categories, int[] categoryIds)
        {
            Categories = categories;
            CategoryIds = categoryIds;
        }

        public int[] CategoryIds { get; set; }
        public List<TreeViewCategory> Categories { get; set; }
    }
}
