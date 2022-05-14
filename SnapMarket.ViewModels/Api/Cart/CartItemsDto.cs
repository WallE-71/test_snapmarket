using System;
using System.Collections.Generic;
using System.Text;

namespace SnapMarket.ViewModels.Api.Cart
{
    public class CartItemDto
    {
        public int Price { get; set; }
        public int Count { get; set; }
        public string Image { get; set; }
        public string ProductId { get; set; }
        public string ProductName { get; set; }       
        public Entities.Product Product { get; set; }
        public virtual List<string> Colors { get; set; }
    }
}
