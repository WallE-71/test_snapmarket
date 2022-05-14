using System.Collections.Generic;

namespace SnapMarket.ViewModels.Cart
{
    public class CartViewModel : BaseViewModel<int>
    {  
        public int ProductCount { get; set; }
        public int SumAmount { get; set; }
        public string Customer { get; set; }
        public virtual List<CartItemViewModel> CartItems { get; set; }
    }
}
