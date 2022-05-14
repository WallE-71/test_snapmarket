using System.Threading.Tasks;
using System.Collections.Generic;
using SnapMarket.ViewModels;

namespace SnapMarket.Data.Contracts
{
    public interface IBrandRepository
    {
        Task<List<BaseViewModel<int>>> GetPaginateBrandsAsync(int offset, int limit, string Orderby, string searchText);
        bool IsExistBrand(string name, int recentId = 0);
        Task<int> InsertProductBrand(string brandName);
    }
}
