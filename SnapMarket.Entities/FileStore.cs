using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SnapMarket.Entities
{
    public class FileStore
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string ImageName { get; set; }
        public DateTime? RemoveTime { get; set; }
        public DateTime? InsertTime { get; set; }

        public virtual string ProductId { get; set; }
        public virtual Product Product { get; set; }
        public virtual int? SellerId { get; set; }
        public virtual Seller Seller { get; set; }
        public virtual int? SliderId { get; set; }
        public virtual Slider Slider { get; set; }
    }
}
