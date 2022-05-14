using System.Threading.Tasks;
using System.Collections.Generic;
using SnapMarket.Entities;
using SnapMarket.ViewModels;
using SnapMarket.ViewModels.Cart;

namespace SnapMarket.Data.Contracts
{
    public interface ICartRepository
    {
        Task<List<CartViewModel>> GetPaginateCartsAsync(int offset, int limit, string orderBy, string searchText);
        Task<List<CartItemViewModel>> GetPaginateCartItemsAsync(int offset, int limit, string orderBy, string searchText, int cartId);

        Task<CartViewModel> GetCartAsync(int userId, string browserId);
        ResultViewModel<CartItem> AddToCart(string productId, string browserId);
        ResultViewModel Increase(int cartItemId);
        ResultViewModel Decrease(int cartItemId);
        ResultViewModel RemoveFromCart(string productId, string browserId);
        ResultViewModel RemoveAllFromCart(string browserId);
    }
}
