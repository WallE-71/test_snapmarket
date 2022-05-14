using System;
using System.Collections.Generic;
using System.Text;

namespace SnapMarket.Entities
{
    public class Guarantee : BaseEntity<int>
    {
        public virtual List<Product> Products { get; set; }
    }
}
