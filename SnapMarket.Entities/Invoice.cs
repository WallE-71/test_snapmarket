using System.Collections.Generic;

namespace SnapMarket.Entities
{
    public class Invoice : BaseEntity<int>
    {
        public virtual ICollection<Order> Orders { get; set; }
    }
}
