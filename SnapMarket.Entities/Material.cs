using System;
using System.Collections.Generic;
using System.Text;

namespace SnapMarket.Entities
{
    public class Material : BaseEntity<int>
    {
        public virtual ICollection<ProductMaterial> ProductMaterials { get; set; }
    }
}
