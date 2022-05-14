using System;
using System.Collections.Generic;

namespace SnapMarket.ViewModels.Api.Cart
{
    public class CartDto
    {  
        public int ProductCount { get; set; }
        public int SumAmount { get; set; }
        public virtual List<CartItemDto> CartItems { get; set; }
    }    
}
