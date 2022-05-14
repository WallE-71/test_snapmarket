using System.Threading.Tasks;
using System.Collections.Generic;
using SnapMarket.ViewModels.Slider;

namespace SnapMarket.Data.Contracts
{
    public interface ISliderRepository
    {
        Task<List<SliderViewModel>> GetPaginateSlidersAsync(int offset, int limit, string orderby, string searchText);
    }
}
