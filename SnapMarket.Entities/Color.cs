using System.Collections.Generic;

namespace SnapMarket.Entities
{
    public class Color : BaseEntity<int>
    {
        public virtual ICollection<ProductColor> ProductColors { get; set; }
    }
}
