using System.Threading.Tasks;
using System.Collections.Generic;
using SnapMarket.ViewModels;

namespace SnapMarket.Data.Contracts
{
    public interface ISellerRepository
    {
        Task<List<SellerViewModel>> GetPaginateSellersAsync(int offset, int limit, string orderBy, string searchText);
        Task<SellerViewModel> GetSellerForProductAsync(string productId);
    }
}
