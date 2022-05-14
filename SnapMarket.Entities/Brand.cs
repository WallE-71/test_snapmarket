using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SnapMarket.Entities
{
    public class Brand : BaseEntity<int>
    {
        public virtual List<Product> Products { get; set; }
    }
}
