using System.Threading.Tasks;
using System.Collections.Generic;
using SnapMarket.ViewModels;
using SnapMarket.ViewModels.Order;

namespace SnapMarket.Data.Contracts
{
    public interface IOrderRepository
    {
        List<OrderViewModel> GetPaginateOrders(int offset, int limit, string orderBy, string searchText);
        Task<ResultViewModel> RequestAddNewOrderAsync(RequestAddNewOrder request);
        Task<ResultViewModel<List<UserOrderViewModel>>> GetUserOrdersAsync(int userId, int offset, int limit);
        ResultViewModel RemoveOrder(int orderId, int userId);       
    }
}
