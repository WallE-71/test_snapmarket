using System.Collections.Generic;
using SnapMarket.Entities.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace SnapMarket.Entities
{
    public class Cart : BaseEntity<int>
    {
        public string BrowserId { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int UserId { get; set; }
        public User User { get; set; }
        public virtual ICollection<CartItem> CartItems { get; set; }     
    }
}
