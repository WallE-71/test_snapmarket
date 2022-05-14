using System;

namespace SnapMarket.Entities
{
    public class CartItem : BaseEntity<int>
    {
        public int Count { get; set; }
        public int Price { get; set; }

        public int CartId { get; set; }
        public virtual Cart Cart { get; set; }
        public string ProductId { get; set; }
        public virtual Product Product { get; set; }
    }
}
