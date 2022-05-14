using SnapMarket.ViewModels.Product;
using System.Collections.Generic;

namespace SnapMarket.ViewModels.Home
{
    public class ProductsPaginateViewModel
    {
        public ProductsPaginateViewModel(int productCount, List<ProductViewModel> products)
        {
            ProductCount = productCount;
            Products = products;           
        }


        public int ProductCount { get; set; }
        public List<ProductViewModel> Products { get; set; }      
    }
}
