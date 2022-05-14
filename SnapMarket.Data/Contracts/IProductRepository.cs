using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using SnapMarket.Entities;
using SnapMarket.ViewModels.Product;
using SnapMarket.ViewModels.Api.Home;

namespace SnapMarket.Data.Contracts
{
    public interface IProductRepository
    {
        Task<List<ProductViewModel>> GetPaginateProductAsync(int offset, int limit, string orderBy, string searchText, bool? IsDemo, bool? isSeller, long sellerId = 0);
        Task<List<ProductInCategoriesDto>> GetProductInCategoryAsync(string categoryName);
        Task<ProductViewModel> GetProductByIdAsync(string productId, int userId);
        Task<List<ProductViewModel>> FindProductByIdAsync(string productId);
        Product FindByProductName(string productName);

        Task<List<ProductViewModel>> MostViewedProductAsync(int offset, int limit, string duration);
        Task<List<ProductViewModel>> MostTalkProduct(int offset, int limit, string duration);
        Task<List<ProductViewModel>> MostPopularProducts(int offset, int limit);    
        Task<List<ProductViewModel>> GetRelatedProductAsync(int number);
        Task<string> GetWeeklyProductDiscountAsync(string Url);
        Task<List<ProductViewModel>> GetUserBookmarksAsync(int userId);
        Task<int> InsertProductGuarantee(string guaranteeName);       
        Task InsertVisitOfUserAsync(string productId, Guid browserId, string ipAddress);
        Task<List<ProductViewModel>> GetPreferedProductsAsync(int number);
        Task<int?> DiscountManagerAsync(string productId, bool save = false);

        int CountProducts();
        int CountFutureProducts();
        int CountProductPresentation();
        int CountProductPresentationOrDemo(bool isDemo);
    }
}
