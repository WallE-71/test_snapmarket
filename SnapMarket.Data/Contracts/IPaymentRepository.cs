using System.Threading.Tasks;
using System.Collections.Generic;
using SnapMarket.ViewModels;
using static SnapMarket.ViewModels.RequestPay.RequestPayViewModel;

namespace SnapMarket.Data.Contracts
{
    public interface IPaymentRepository
    {
        Task<List<ItemRequestPay>> GetPaginateRequestPaysAsync(int offset, int limit, bool? sortAsc, string searchText);
        ResultViewModel<ItemRequestPay> GetRequestPay(string requestPayId);
        ResultViewModel<ResultRequestPay> AddRequestPay(int amount, int userId, string discountCode, Entities.TransportType transportType);
    }
}
