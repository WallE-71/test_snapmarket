using System.Threading.Tasks;
using System.Collections.Generic;
using SnapMarket.Entities;
using SnapMarket.ViewModels;

namespace SnapMarket.Data.Contracts
{
    public interface IColorRepository
    {
        Task<List<BaseViewModel<int>>> GetPaginateColorsAsync(int offset, int limit, string orderby, string searchText);
        bool IsExistColor(string name, int recentId = 0);
        Task<ICollection<ProductColor>> InsertProductColors(string[] colors, string primaryColor, string productId = null);
    }
}
