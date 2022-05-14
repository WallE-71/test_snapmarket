using System;
using SnapMarket.Entities.Identity;

namespace SnapMarket.Entities
{
    public class Bookmark
    {
        public DateTime? RemoveTime { get; set; }
        public DateTime? InsertTime { get; set; }

        public int UserId { get; set; }
        public virtual User User { get; set; }
        public string ProductId { get; set; }
        public virtual Product Product { get; set; }
    }
}
