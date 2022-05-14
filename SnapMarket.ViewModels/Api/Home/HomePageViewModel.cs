using System.Collections.Generic;
using SnapMarket.Entities.Identity;
using SnapMarket.ViewModels.Product;

namespace SnapMarket.ViewModels.Home
{
    public class HomePageViewModel
    {
        public HomePageViewModel(User _user, List<ProductViewModel> products, bool requiresTwoFactor = false)
        {
            user = _user;
            Products = products;
            user.TwoFactorEnabled = requiresTwoFactor;
        }


        public User user { get; set; }
        public List<ProductViewModel> Products { get; set; }
    }
}
