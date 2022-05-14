using System;
using System.Collections.Generic;

namespace SnapMarket.Entities
{
    public class ProductMaterial
    {
        public string ProductId { get; set; }
        public Product Product { get; set; }

        public int MaterialId { get; set; }
        public Material Material { get; set; }
    }
}
