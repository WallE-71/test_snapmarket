using System.Threading.Tasks;
using System.Collections.Generic;
using SnapMarket.Entities;
using SnapMarket.ViewModels;

namespace SnapMarket.Data.Contracts
{
    public interface IMaterialRepository
    {
        Task<List<BaseViewModel<int>>> GetPaginateMaterialsAsync(int offset, int limit, string orderby, string searchText);
        bool IsExistMaterial(string name, int recentId = 0);
        Task<ICollection<ProductMaterial>> InsertProductMaterials(string[] materials, string productId = null);
    }
}
