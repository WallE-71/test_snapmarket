using System.Threading.Tasks;
using System.Collections.Generic;
using SnapMarket.ViewModels;

namespace SnapMarket.Data.Contracts
{
    public interface ICreditCartRepository
    {
        Task<List<CreditCartViewModel>> GetPaginateCreditCartsAsync(int offset, int limit, string orderBy, string searchText);
    }
}
